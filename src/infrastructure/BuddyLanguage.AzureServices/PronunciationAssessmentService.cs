using BuddyLanguage.Domain.Entities;
using BuddyLanguage.Domain.Exceptions.AzureService;
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
    private readonly PronunciationAssessmentConfig _pronunciationAssessmentConfig;
    private readonly SpeechConfig _speechConfig;

    public PronunciationAssessmentService(
        IOptions<AzureConfig> config,
        ILogger<PronunciationAssessmentService> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        AzureConfig azureConfig = config.Value ?? throw new ArgumentNullException(nameof(config));

        // Объявление конфигурации сервиса оценки произношения
        _pronunciationAssessmentConfig = new PronunciationAssessmentConfig(
            referenceText: string.Empty,
            gradingSystem: GradingSystem.HundredMark,
            granularity: Granularity.Phoneme,
            enableMiscue: false) { NBestPhonemeCount = 5 };

        _speechConfig = SpeechConfig.FromSubscription(
            azureConfig.SpeechKey,
            azureConfig.SpeechRegion);
        _speechConfig.SpeechRecognitionLanguage = "en-US";
    }

    /// <summary>
    /// Получить оценку произношения
    /// </summary>
    /// <param name="voiceMessage">Голосовое сообщение в виде набора бойт.
    /// Поддерживаемые форматы: wav
    /// </param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Оценка произношения для каждого слова</returns>
    public async Task<IReadOnlyList<WordPronunciationAssessment>> GetSpeechAssessmentAsync(
        byte[] voiceMessage,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(voiceMessage);

        if (voiceMessage.Length == 0)
        {
            throw new ArgumentException(
                "Поток аудиоданных не может быть пустым.",
                nameof(voiceMessage));
        }

        // Подготовка потока данных к обработке сервисом
        var ms = new MemoryStream(voiceMessage);
        var reader = new BinaryReader(ms);
        using var audioConfigStream = AudioInputStream.CreatePushStream();
        using var audioConfig = AudioConfig.FromStreamInput(audioConfigStream);
        using var speechRecognizer = new SpeechRecognizer(_speechConfig, audioConfig);
        _pronunciationAssessmentConfig.ApplyTo(speechRecognizer);
        byte[] readBytes;
        do
        {
            readBytes = reader.ReadBytes(1024);
            audioConfigStream.Write(readBytes, readBytes.Length);
        }
        while (readBytes.Length > 0);

        var speechRecognitionResult = await speechRecognizer.RecognizeOnceAsync();

        if (speechRecognitionResult.Reason != ResultReason.RecognizedSpeech)
        {
            throw new SpeechNotRecognizedException(
                $"Не удалось распознать речь. Ошибка: {speechRecognitionResult.Reason} " +
                $"Детали: {speechRecognitionResult}");
        }

        // Получение результата оценки произношения
        var pronunciationAssessmentResult =
            PronunciationAssessmentResult.FromResult(speechRecognitionResult);

        var totalResult =
            pronunciationAssessmentResult
            .Words
            .Select(word => new WordPronunciationAssessment(word.Word, word.AccuracyScore))
            .ToList();

        return totalResult;
    }

    public async Task<IReadOnlyList<WordPronunciationAssessment>> PronunciationAssessmentWithStreamInternalAsync(
        byte[] audioData,
        CancellationToken cancellationToken)
    {
        using (var audioInputStream = AudioInputStream.CreatePushStream(AudioStreamFormat.GetWaveFormatPCM(16000, 16, 1))) // This need be set based on the format of the given audio data
        using (var audioConfig = AudioConfig.FromStreamInput(audioInputStream))

        // Specify the language used for Pronunciation Assessment.
        using (var speechRecognizer = new SpeechRecognizer(_speechConfig, "en-US", audioConfig))
        {
            _pronunciationAssessmentConfig.ApplyTo(speechRecognizer);

            audioInputStream.Write(audioData);
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
}
