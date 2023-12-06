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
    private readonly ILogger<PronunciationAssessmentService> _logger;
    private readonly INAudioOggToPcmConverter _nAudioOggToWavConverter;
    private readonly PronunciationAssessmentConfig _pronunciationAssessmentConfig;
    private readonly SpeechConfig _speechConfig;

    public PronunciationAssessmentService(
        IOptions<AzureConfig> config,
        ILogger<PronunciationAssessmentService> logger,
        INAudioOggToPcmConverter nAudioOggToWavConverter)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        AzureConfig azureConfig = config.Value ?? throw new ArgumentNullException(nameof(config));

        // Объявление конфигурации сервиса оценки произношения
        _pronunciationAssessmentConfig = new PronunciationAssessmentConfig(
            referenceText: string.Empty,
            gradingSystem: GradingSystem.HundredMark,
            granularity: Granularity.Phoneme,
            enableMiscue: false)
        { NBestPhonemeCount = 5 };

        _speechConfig = SpeechConfig.FromSubscription(
            azureConfig.SpeechKey,
            azureConfig.SpeechRegion);
        _nAudioOggToWavConverter = nAudioOggToWavConverter
            ?? throw new ArgumentNullException(nameof(nAudioOggToWavConverter));
    }

    /// <summary>
    /// Получить оценку произношения
    /// </summary>
    /// <param name="audioData">Голосовое сообщение в виде набора бойт.
    /// Поддерживаемые форматы: PCM, 16 bit, sample rate 16000, mono
    /// </param>
    /// <param name="targetLanguage">язык пользователя
    /// Для английского языка задать "en-US"</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Оценка произношения для каждого слова</returns>
    public async Task<IReadOnlyList<WordPronunciationAssessment>> GetSpeechAssessmentAsync(
        byte[] audioData,
        Language targetLanguage,
        CancellationToken cancellationToken)
    {
        var language = GetLanguageFromEnum(targetLanguage);
        var audioDataPCM = _nAudioOggToWavConverter.ConvertOggToPcm(audioData);

        // Specify exact language to recognizer
        _speechConfig.SpeechRecognitionLanguage = language;

        using (var audioInputStream = AudioInputStream.CreatePushStream(AudioStreamFormat.GetWaveFormatPCM(16000, 16, 1))) // This need be set based on the format of the given audio data
        using (var audioConfig = AudioConfig.FromStreamInput(audioInputStream))

        // Specify the language used for Pronunciation Assessment.
        using (var speechRecognizer = new SpeechRecognizer(_speechConfig, language, audioConfig))
        {
            _pronunciationAssessmentConfig.ApplyTo(speechRecognizer);

            audioInputStream.Write(audioDataPCM);
            audioInputStream.Write(new byte[0]); // send a zero-size chunk to signal the end of stream

            var result = await speechRecognizer.RecognizeOnceAsync().ConfigureAwait(false);

            if (result.Reason == ResultReason.Canceled)
            {
                var cancellationDetail = CancellationDetails.FromResult(result);
                throw cancellationDetail.Reason switch
                {
                    CancellationReason.Error => new Exception(
                        $"CANCELED: ErrorCode={cancellationDetail.ErrorCode} ErrorDetails={cancellationDetail.ErrorDetails}"),
                    CancellationReason.EndOfStream => new Exception(
                        $"CANCELED: ReachedEndOfStream={cancellationDetail.ErrorDetails}"),
                    _ => throw new Exception($"CANCELED: Reason={cancellationDetail.Reason} ErrorDetails={cancellationDetail.ErrorDetails}"),
                };
            }

            // Получение результата оценки произношения
            var pronunciationAssessmentResult =
                PronunciationAssessmentResult.FromResult(result);

            var totalResult =
                pronunciationAssessmentResult
                    .Words
                    .Select(word => new WordPronunciationAssessment(word.Word, word.AccuracyScore))
                    .ToList();

            return totalResult;
        }
    }

    private string GetLanguageFromEnum(Language language)
    {
        return language switch
        {
            Language.Russian => "ru-RU",
            Language.English => "en-US",
            _ => throw new NotSupportedException(
                "The Language You Provided Is Not Currently Supported By Our Project!")
        };
    }
}
