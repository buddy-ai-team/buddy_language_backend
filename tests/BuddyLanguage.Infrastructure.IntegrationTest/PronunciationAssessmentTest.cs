using BuddyLanguage.AzureServices;
using BuddyLanguage.Domain.Entities;
using BuddyLanguage.Domain.Enumerations;
using BuddyLanguage.NAudioConcentusOggOpusToPcmConverterLib;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NAudio.Wave;

namespace BuddyLanguage.Infrastructure.IntegrationTest;

public class PronunciationAssessmentTest
{
    private readonly IOptions<AzureConfig> _config;
    private readonly ILogger<PronunciationAssessmentService> _logger;
    private readonly PronunciationAssessmentService _service;
    private readonly NAudioConcentusOggOpusToPcmConverter _oggOpusToPcmConverter;

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
            SpeechKey = speechKey, SpeechRegion = speechRegion
        });

        _logger = new LoggerFactory().CreateLogger<PronunciationAssessmentService>();

        _oggOpusToPcmConverter = new NAudioConcentusOggOpusToPcmConverter();
        _service = new PronunciationAssessmentService(_config, _logger, _oggOpusToPcmConverter);
    }

    [Fact]
    public async Task Rodions_bad_pronunciation_assessment_found_bad_words()
    {
        // Arrange
        byte[] inputData = await File.ReadAllBytesAsync("assets/bad_pronunciation_sample_5_words.ogg");

        // Act
        var result = await _service.GetSpeechAssessmentFromOggOpus(inputData, Language.English, default);

        // Assert
        result.Should().HaveCount(5);
    }

    [Fact]
    public async Task Christinas_bad_pronunciation_assessment_found_bad_words2()
    {
        // Arrange
        byte[] inputData = await File.ReadAllBytesAsync("assets/bad_pronunciation_sample_3_words.ogg");

        // Act
        var result = await _service.GetSpeechAssessmentFromOggOpus(inputData, Language.English, default);

        // Assert
        result.Should().HaveCount(3); //90, 92, 65
    }
}
