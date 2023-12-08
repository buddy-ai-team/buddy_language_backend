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
            granularity: Granularity.Phoneme,
            enableMiscue: false) { NBestPhonemeCount = 5 };
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

    public async Task<IReadOnlyList<WordPronunciationAssessment>>
        GetSpeechAssessmentFromRawPcmAsync(
            byte[] audioDataPcm,
            Language targetLanguage,
            CancellationToken cancellationToken)
    {
        var language = GetLanguageFromEnum(targetLanguage);
        _speechConfig.SpeechRecognitionLanguage = language;
        using (var audioInputStream =
               AudioInputStream.CreatePushStream(AudioStreamFormat.GetWaveFormatPCM(16000, 16, 1)))
        using (var audioConfig = AudioConfig.FromStreamInput(audioInputStream))
        using (var speechRecognizer = new SpeechRecognizer(_speechConfig, language, audioConfig))
        {
            _pronunciationAssessmentConfig.ApplyTo(speechRecognizer);
            audioInputStream.Write(audioDataPcm);
            audioInputStream.Write(new byte[0]);
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
                    _ => throw new Exception(
                        $"CANCELED: Reason={cancellationDetail.Reason} ErrorDetails={cancellationDetail.ErrorDetails}"),
                };
            }

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
