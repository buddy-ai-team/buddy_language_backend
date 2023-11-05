using BuddyLanguage.Domain.Entities;
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
    private readonly AzureConfig _config;
    private readonly PronunciationAssessmentConfig _pronunciationAssessmentConfig;
    private readonly SpeechConfig _speechConfig;

    public PronunciationAssessmentService(
        IOptions<AzureConfig> config,
        ILogger<PronunciationAssessmentService> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _config = config.Value ?? throw new ArgumentNullException(nameof(config));

        // Объявление конфигурации сервиса оценки произношения
        _pronunciationAssessmentConfig = new PronunciationAssessmentConfig(
            referenceText: string.Empty,
            gradingSystem: GradingSystem.HundredMark,
            granularity: Granularity.Phoneme,
            enableMiscue: false) { NBestPhonemeCount = 5 };

        _speechConfig = SpeechConfig.FromSubscription(_config.SpeechKey, _config.SpeechRegion);
        _speechConfig.SpeechRecognitionLanguage = "en-US";
    }

    /// <summary>
    /// Получить оценку произношения
    /// </summary>
    /// <param name="voiceMessage">Голосовое сообщение в виде набора бойт.
    /// Поддерживаемые форматы: ogg, wav
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
        PushAudioInputStream audioConfigStream = AudioInputStream.CreatePushStream();
        audioConfigStream.Write(voiceMessage, voiceMessage.Length);
        var audioConfig = AudioConfig.FromStreamInput(audioConfigStream);

        var speechRecognizer = new SpeechRecognizer(_speechConfig, audioConfig);
        _pronunciationAssessmentConfig.ApplyTo(speechRecognizer);

        // Запуск обработки потока данных
        SpeechRecognitionResult speechRecognitionResult =
            await speechRecognizer.RecognizeOnceAsync();

        // Получение результата оценки произношения
        var pronunciationAssessmentResult =
            PronunciationAssessmentResult.FromResult(speechRecognitionResult);

        var result =
            pronunciationAssessmentResult
            .Words
            .Select(word => new WordPronunciationAssessment(word.Word, word.AccuracyScore))
            .ToList();

        return result;
    }
}
