using BuddyLanguage.AzureServices;
using BuddyLanguage.Domain.Entities;
using FluentAssertions;
using Microsoft.CognitiveServices.Speech;
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
        // Arrange{
        const string fileName = "assets/Pronunciation.wav";
        byte[] bytes = await File.ReadAllBytesAsync(fileName);

        var service = new PronunciationAssessmentService(_config, _logger);

        //Act
        IReadOnlyList<WordPronunciationAssessment> result =
            await service.GetSpeechAssessmentAsync(bytes, default);

        // Assert
        result.Count.Should().NotBe(0);
    }
}
