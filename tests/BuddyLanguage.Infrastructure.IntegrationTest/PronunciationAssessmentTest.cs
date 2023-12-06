using BuddyLanguage.AzureServices;
using BuddyLanguage.Domain.Entities;
using BuddyLanguage.Domain.Enumerations;
using BuddyLanguage.Domain.Interfaces;
using BuddyLanguage.NAudioOggToWavConverter;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BuddyLanguage.Infrastructure.IntegrationTest;

public class PronunciationAssessmentTest
{
    private readonly IOptions<AzureConfig> _config;
    private readonly ILogger<PronunciationAssessmentService> _logger;

    public PronunciationAssessmentTest()
    {
        string? speechKey = Environment.GetEnvironmentVariable("AZURE_SPEECH_KEY");
        if (speechKey is null)
        {
            throw new ArgumentNullException(nameof(speechKey));
        }

        string? speechRegion = Environment.GetEnvironmentVariable("AZURE_SPEECH_REGION");
        if (speechRegion is null)
        {
            throw new ArgumentNullException(nameof(speechRegion));
        }

        _config = Options.Create(new AzureConfig
        {
            SpeechKey = speechKey,
            SpeechRegion = speechRegion
        });

        _logger = new LoggerFactory().CreateLogger<PronunciationAssessmentService>();
    }

    [Fact]
    public async Task Result_of_assessment_calculated()
    {
        // Arrange
        byte[] inputData = File.ReadAllBytes("assets/Pronunciation.ogg");
        NAudioOggToPcmConverter converter = new NAudioOggToPcmConverter();
        var service = new PronunciationAssessmentService(_config, _logger, converter);

        // Act
        IReadOnlyList<WordPronunciationAssessment> result =
            await service.GetSpeechAssessmentAsync(
                inputData, Language.English, default);

        // Assert
        result.Count.Should().BeGreaterThan(3);
    }
}
