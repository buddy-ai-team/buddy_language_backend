using BuddyLanguage.Domain.Entities;
using BuddyLanguage.Domain.Enumerations;
using BuddyLanguage.Domain.Interfaces;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using Microsoft.CognitiveServices.Speech.PronunciationAssessment;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BuddyLanguage.AzureServices;

public class PronunciationAssessmentService : IPronunciationAssessmentService
{
    // TODO: move to BuddyConfig
    private static readonly TimeSpan RecognitionTimeout = TimeSpan.FromMinutes(2);

    private static readonly Dictionary<Language, string> LanguagesBCP47 = new()
    {
        [Language.Russian] = "ru-RU",
        [Language.English] = "en-US",
        [Language.Arabic] = "ar-SA",
        [Language.ChineseTraditional] = "zh-TW",
        [Language.ChineseSimplified] = "zh-CN",
        [Language.EnglishAustralia] = "en-AU",
        [Language.EnglishCanada] = "en-CA",
        [Language.EnglishIndia] = "en-IN",
        [Language.EnglishUnitedKingdom] = "en-GB",
        [Language.French] = "fr-FR",
        [Language.FrenchCanada] = "fr-CA",
        [Language.GermanGermany] = "de-DE",
        [Language.HindiIndia] = "hi-IN",
        [Language.TamilIndia] = "ta-IN",
        [Language.Italian] = "it-IT",
        [Language.Japanese] = "ja-JP",
        [Language.Korean] = "ko-KR",
        [Language.Malay] = "ms-MY",
        [Language.Norwegian] = "no-NO",
        [Language.Portuguese] = "pt-PT",
        [Language.Spanish] = "es-ES",
        [Language.SpanishMexico] = "es-MX",
        [Language.Swedish] = "sv-SE",
        [Language.Vietnamese] = "vi-VN",
    };

    private readonly ILogger<PronunciationAssessmentService> _logger;
    private readonly IOggOpusToPcmConverter _oggToWavConverter;
    private readonly AzureConfig _azureConfig;

    public PronunciationAssessmentService(
        IOptions<AzureConfig> config,
        ILogger<PronunciationAssessmentService> logger,
        IOggOpusToPcmConverter oggOpusToPcmConverter)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _azureConfig = config.Value ?? throw new ArgumentNullException(nameof(config));
        _oggToWavConverter = oggOpusToPcmConverter
                             ?? throw new ArgumentNullException(nameof(oggOpusToPcmConverter));
    }

    // https://learn.microsoft.com/en-us/azure/ai-services/speech-service/language-support?tabs=pronunciation-assessment#supported-languages
    public bool IsLanguageSupported(Language language)
        => LanguagesBCP47.ContainsKey(language);

    public async Task<IReadOnlyList<WordPronunciationAssessment>> GetSpeechAssessmentFromOggOpus(
        byte[] audioData,
        Language targetLanguage,
        CancellationToken cancellationToken)
    {
        var audioDataPcm = await _oggToWavConverter.ConvertOggToPcm(audioData);
        return await GetSpeechAssessmentFromRawPcmAsync(audioDataPcm, targetLanguage, cancellationToken);
    }

    // More examples of this module usage: https://github.com/Azure-Samples/cognitive-services-speech-sdk/blob/master/samples/csharp/sharedcontent/console/speech_recognition_samples.cs
    private async Task<IReadOnlyList<WordPronunciationAssessment>> GetSpeechAssessmentFromRawPcmAsync(
        byte[] audioDataPcm,
        Language targetLanguage,
        CancellationToken cancellationToken)
    {
        var (language, speechConfig, pronunciationConfig) = GetConfigs(targetLanguage);
        var stopRecognitionTcs = new TaskCompletionSource<int>(TaskCreationOptions.RunContinuationsAsynchronously);

        using var audioInputStream = AudioInputStream.CreatePushStream(AudioStreamFormat.GetWaveFormatPCM(16000, 16, 1));
        using var audioConfig = AudioConfig.FromStreamInput(audioInputStream);
        using var recognizer = new SpeechRecognizer(speechConfig, language, audioConfig);
        pronunciationConfig.ApplyTo(recognizer);

        var totalResults = new List<WordPronunciationAssessment>();

        // More examples that are using events: https://github.com/Azure-Samples/cognitive-services-speech-sdk/blob/master/samples/csharp/sharedcontent/console/speech_recognition_samples.cs#L1146
        recognizer.Recognized += OnRecognized;
        recognizer.Canceled += OnCanceled;
        recognizer.SessionStarted += (_, _) => _logger.LogDebug("Session started");
        recognizer.SpeechStartDetected += (_, _) => _logger.LogDebug("Speech start detected");
        recognizer.SpeechEndDetected += (_, _) => _logger.LogDebug("Speech end detected");
        recognizer.Recognizing += (_, _) => _logger.LogDebug("Recognizing");
        recognizer.SessionStopped += (_, _) =>
        {
            _logger.LogDebug("Session stopped");
            stopRecognitionTcs.TrySetResult(0);
        };

        audioInputStream.Write(audioDataPcm);
        audioInputStream.Write(Array.Empty<byte>());

        await recognizer.StartContinuousRecognitionAsync();

        await stopRecognitionTcs.Task.WaitAsync(RecognitionTimeout, cancellationToken);

        await recognizer.StopContinuousRecognitionAsync();

        return totalResults.AsReadOnly();

        void OnRecognized(object? sender, SpeechRecognitionEventArgs e)
        {
            switch (e.Result.Reason)
            {
                case ResultReason.RecognizedSpeech:
                    {
                        // for English language we can simply take the worst phoneme
                        var result = PronunciationAssessmentResult.FromResult(e.Result);
                        var assessments = result.Words
                            .Select(it => new WordPronunciationAssessment(it.Word, it.AccuracyScore));
                        totalResults.AddRange(assessments);
                        break;
                    }

                case ResultReason.NoMatch:
                    _logger.LogWarning("NOMATCH: Speech could not be recognized");
                    break;
                default:
                    _logger.LogError("Unhandled recognition result: {Reason}", e.Result.Reason);
                    break;
            }
        }

        void OnCanceled(object? sender, SpeechRecognitionCanceledEventArgs e)
        {
            switch (e.Reason)
            {
                case CancellationReason.Error:
                    stopRecognitionTcs.SetException(new OperationCanceledException(
                        $"Regognition error. ErrorCode={e.ErrorCode} ErrorDetails={e.ErrorDetails}"));
                    break;
                case CancellationReason.EndOfStream:
                    _logger.LogInformation("End of stream reached");
                    stopRecognitionTcs.TrySetResult(0);
                    break;
                case CancellationReason.CancelledByUser:
                    _logger.LogInformation("Cancelled by user");
                    stopRecognitionTcs.SetException(new OperationCanceledException(
                        $"Cancelled by user. ErrorCode={e.ErrorCode} ErrorDetails={e.ErrorDetails}"));
                    break;
                default:
                    stopRecognitionTcs.SetException(new InvalidOperationException(
                        $"Unhandled cancellation reason: {e.Reason}. ErrorCode={e.ErrorCode} ErrorDetails={e.ErrorDetails}"));
                    break;
            }
        }
    }

    private Configs GetConfigs(Language targetLanguage)
    {
        /*
        PhonemeAlphabet
        Понимание алфавита фонем: Фонемы - это мельчайшие звуковые единицы в языке. Способы представления и классификации этих звуков могут быть разными. В вашем коде SDK позволяет выбирать между двумя алфавитами фонем: SAPI (Speech API) и IPA (International Phonetic Alphabet).

        SAPI против IPA:

        SAPI: Это набор фонем, используемый речевыми движками Microsoft. Он разработан для повышения эффективности вычислений и интегрирован с другими речевыми технологиями Microsoft. Однако он может быть не таким универсальным и точным, как IPA.
        IPA: Международный фонетический алфавит - это общепризнанная система представления звуков разговорного языка. Он более подробный и используется во всем мире лингвистами, логопедами и преподавателями языка. Он позволяет более точно отображать широкий спектр звуков.
        Когда использовать алфавит:

        Для приложений, работающих с несколькими языками или требующих детального фонетического анализа, IPA обычно является лучшим выбором благодаря своей точности и универсальности.
        Для приложений, тесно интегрированных с экосистемой Microsoft или ориентированных в основном на языки, хорошо поддерживаемые SAPI, использование SAPI может быть более эффективным.
        Влияние на точность: Выбор правильного алфавита фонем может повысить точность распознавания фонем, особенно для языков или диалектов с нюансами фонетических вариаций.

        NBestPhonemeCount
        Понимание свойства NBestPhonemeCount: Это свойство определяет количество фонетических интерпретаций (фонем), которые система рассматривает для каждого сегмента речи. Большее число означает, что система исследует больше возможностей, прежде чем принять решение о наилучшем соответствии.

        Настройка подсчета:

        Увеличение этого значения может повысить точность в тех случаях, когда правильное произношение не является наиболее очевидным для системы. Это может быть особенно полезно для дикторов с акцентом или дефектами речи.
        Однако увеличение количества вариантов может также увеличить время обработки и усложнить систему за счет рассмотрения слишком большого количества альтернатив, некоторые из которых могут быть неправдоподобными.
        Баланс между производительностью и точностью: Поиск правильного значения для NBestPhonemeCount предполагает баланс между эффективностью вычислений и необходимостью детального фонетического анализа. Возможно, вам придется поэкспериментировать с различными значениями, чтобы понять, что лучше всего подходит для вашего конкретного случая использования.

        Учет специфики использования: Для общих случаев использования может быть достаточно умеренного значения. Однако для специализированных приложений, таких как инструменты для изучения языка, логопедическое программное обеспечение или системы, используемые не носителями языка, более высокое значение может быть более полезным для улавливания тонких нюансов произношения.

        Общие рекомендации
        Тестирование и итерации: Обе настройки следует протестировать на различных образцах речи. Проанализируйте результаты и проведите итерации, чтобы найти оптимальную конфигурацию.
        Отзывы пользователей: По возможности собирайте отзывы пользователей о точности оценки произношения. Это может дать ценную информацию о том, насколько хорошо работает система.
        Следите за обновлениями: Следите за обновлениями SDK, поскольку в новых версиях могут появиться улучшения или дополнительные функции, связанные с обработкой фонем и оценкой произношения.
        */

        var language = GetLanguageFromEnum(targetLanguage);
        var speechConfig = SpeechConfig.FromSubscription(
            _azureConfig.SpeechKey, _azureConfig.SpeechRegion);
        speechConfig.SpeechRecognitionLanguage = language;
        speechConfig.OutputFormat = OutputFormat.Detailed;

        var pronunciationAssessmentConfig = new PronunciationAssessmentConfig(
            referenceText: string.Empty,
            gradingSystem: GradingSystem.HundredMark,
            granularity: Granularity.Word,
            enableMiscue: true)
        {
            NBestPhonemeCount = 5,
        };

        return new Configs(language, speechConfig, pronunciationAssessmentConfig);
    }

    private string GetLanguageFromEnum(Language language)
    {
        if (!LanguagesBCP47.TryGetValue(language, out string? value))
        {
            throw new NotSupportedException($"{language} Language Is Not Currently Supported By Our Project!");
        }

        return value;
    }

    private record struct Configs(
        string Language,
        SpeechConfig SpeechConfig,
        PronunciationAssessmentConfig PronunciationAssessmentConfig);
}
