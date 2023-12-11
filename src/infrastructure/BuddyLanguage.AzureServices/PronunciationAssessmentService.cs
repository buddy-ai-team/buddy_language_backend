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
    private readonly IOggOpusToPcmConverter _oggToWavConverter;
    private readonly PronunciationAssessmentConfig _pronunciationAssessmentConfig;
    private readonly SpeechConfig _speechConfig;

    public PronunciationAssessmentService(
        IOptions<AzureConfig> config,
        ILogger<PronunciationAssessmentService> logger,
        IOggOpusToPcmConverter oggOpusToPcmConverter)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        AzureConfig azureConfig = config.Value ?? throw new ArgumentNullException(nameof(config));
        _pronunciationAssessmentConfig = new PronunciationAssessmentConfig(
            referenceText: string.Empty,
            gradingSystem: GradingSystem.HundredMark,
            granularity: Granularity.Word,
            enableMiscue: true) { NBestPhonemeCount = 5 };
        _speechConfig = SpeechConfig.FromSubscription(
            azureConfig.SpeechKey,
            azureConfig.SpeechRegion);
        _oggToWavConverter = oggOpusToPcmConverter
                             ?? throw new ArgumentNullException(nameof(oggOpusToPcmConverter));
    }

    public async Task<IReadOnlyList<WordPronunciationAssessment>> GetSpeechAssessmentFromOggAsync(
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
        var language = GetLanguageFromEnum(targetLanguage);
        _speechConfig.SpeechRecognitionLanguage = language;
        var stopRecognition = new TaskCompletionSource<int>();

        using (var audioInputStream = AudioInputStream.CreatePushStream(AudioStreamFormat.GetWaveFormatPCM(16000, 16, 1)))
        using (var audioConfig = AudioConfig.FromStreamInput(audioInputStream))
        using (var recognizer = new SpeechRecognizer(_speechConfig, language, audioConfig))
        {
            _pronunciationAssessmentConfig.ApplyTo(recognizer);

            recognizer.Recognizing += (s, e) =>
            {
                // Handle intermediate recognition results if needed
            };

            var totalResults = new List<WordPronunciationAssessment>();

            recognizer.Recognized += (s, e) =>
            {
                if (e.Result.Reason == ResultReason.RecognizedSpeech)
                {
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

            recognizer.Canceled += (s, e) =>
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

                stopRecognition.TrySetResult(0);
            };

            recognizer.SessionStarted += (s, e) =>
            {
                // Handle session start if needed
            };

            recognizer.SessionStopped += (s, e) =>
            {
                stopRecognition.TrySetResult(0);
            };

            audioInputStream.Write(audioDataPcm);
            audioInputStream.Write(Array.Empty<byte>());

            await recognizer.StartContinuousRecognitionAsync();

            // Waits for completion.
            await stopRecognition.Task.WaitAsync(TimeSpan.FromSeconds(30), cancellationToken);

            await recognizer.StopContinuousRecognitionAsync();

            return totalResults.AsReadOnly();
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
