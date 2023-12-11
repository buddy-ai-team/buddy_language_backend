using BuddyLanguage.AzureServices;
using BuddyLanguage.Domain.Entities;
using BuddyLanguage.Domain.Enumerations;
using BuddyLanguage.NAudioMediaFoundationOggOpusToPcmConverterLib;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NAudio.Wave;

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
            SpeechKey = speechKey, SpeechRegion = speechRegion
        });

        _logger = new LoggerFactory().CreateLogger<PronunciationAssessmentService>();
    }

    [Fact]
    public async Task Pronunciation_assessment_succeeded()
    {
        // Arrange
        byte[] inputData = await File.ReadAllBytesAsync("assets/bad_pronunciation_sample_4_words.ogg");
        var oggOpusToPcmConverter = new NAudioMediaFoundationOggOpusToPcmConverter();
        var service = new PronunciationAssessmentService(_config, _logger, oggOpusToPcmConverter);

        // Act
        IReadOnlyList<WordPronunciationAssessment> result =
            await service.GetSpeechAssessmentFromOggAsync(
                inputData, Language.English, default);

        // Assert
        result.Should().HaveCount(4);
    }

    private class PcmToWavConverter
    {
        public void ConvertPcmToWav(byte[] pcmData, string outputPath, int sampleRate = 48000, int channels = 1, int bitsPerSample = 16)
        {
            using var waveFile = new WaveFileWriter(outputPath, new WaveFormat(sampleRate, bitsPerSample, channels));
            waveFile.Write(pcmData, 0, pcmData.Length);
        }
    }
}
