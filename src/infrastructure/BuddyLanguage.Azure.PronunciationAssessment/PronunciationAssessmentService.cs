using BuddyLanguage.Domain.Entities;
using BuddyLanguage.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace BuddyLanguage.Azure.SpeechAssessment;

public class PronunciationAssessmentService : IPronunciationAssessmentService
{
    private readonly ILogger<PronunciationAssessmentService> _logger;
    private readonly AzureTTSConfig _config;

    public PronunciationAssessmentService(IOptions<AzureTTSConfig> config, ILogger<PronunciationAssessmentService> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _config = config.Value ?? throw new ArgumentNullException(nameof(config));
    }


    public async Task<IReadOnlyList<WordPronunciationAssessment>> GetSpeechAssessmentAsync(
        byte[] voiceMessage,
        CancellationToken cancellationToken)
    {
        WordPronunciationAssessment[];
    }
}
