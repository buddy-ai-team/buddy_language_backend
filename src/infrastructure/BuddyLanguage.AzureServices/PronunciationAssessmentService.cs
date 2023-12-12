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

    private async Task<IReadOnlyList<WordPronunciationAssessment>> GetSpeechAssessmentFromRawPcmAsync(
        byte[] audioDataPcm,
        Language targetLanguage,
        CancellationToken cancellationToken)
    {
        var (language, speechConfig, pronunciationConfig) = GetConfigs(targetLanguage);
        var stopRecognitionTcs = new TaskCompletionSource<int>();

        using var audioInputStream = AudioInputStream.CreatePushStream(AudioStreamFormat.GetWaveFormatPCM(16000, 16, 1));
        using var audioConfig = AudioConfig.FromStreamInput(audioInputStream);
        using var recognizer = new SpeechRecognizer(speechConfig, language, audioConfig);
        pronunciationConfig.ApplyTo(recognizer);

        recognizer.Recognizing += (_, _) =>
        {
            // Handle intermediate recognition results if needed
        };

        var totalResults = new List<WordPronunciationAssessment>();

        recognizer.Recognized += (_, e) =>
        {
            if (e.Result.Reason == ResultReason.RecognizedSpeech)
            {
                // для англ. языка можно просто брать худшию фонему
                var pronunciationAssessmentResult = PronunciationAssessmentResult.FromResult(e.Result);
                totalResults.AddRange(
                    pronunciationAssessmentResult.Words
                        .Select(word => new WordPronunciationAssessment(word.Word, word.AccuracyScore)));
            }
            else if (e.Result.Reason == ResultReason.NoMatch)
            {
                _logger.LogWarning("NOMATCH: Speech could not be recognized");
            }
            else
            {
                _logger.LogError("Unhandled recognition result: {Reason}", e.Result.Reason);
            }
        };

        recognizer.Canceled += (_, e) =>
        {
            if (e.Reason == CancellationReason.Error)
            {
                throw new InvalidOperationException(
                    $"CANCELED: ErrorCode={e.ErrorCode} ErrorDetails={e.ErrorDetails}");
            }

            if (e.Reason == CancellationReason.EndOfStream)
            {
                _logger.LogInformation("End of stream reached");
            }

            stopRecognitionTcs.TrySetResult(0);
        };

        recognizer.SessionStarted += (_, _) =>
        {
            // Handle session start if needed
        };

        recognizer.SessionStopped += (_, _) =>
        {
            stopRecognitionTcs.TrySetResult(0);
        };

        audioInputStream.Write(audioDataPcm);
        audioInputStream.Write(Array.Empty<byte>());

        await recognizer.StartContinuousRecognitionAsync();

        // Waits for completion.
        await stopRecognitionTcs.Task.WaitAsync(TimeSpan.FromSeconds(30), cancellationToken);

        await recognizer.StopContinuousRecognitionAsync();

        return totalResults.AsReadOnly();
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
