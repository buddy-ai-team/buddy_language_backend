using BuddyLanguage.AzureServices;
using BuddyLanguage.Domain.Entities;
using BuddyLanguage.Domain.Enumerations;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NAudio.Vorbis;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;

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
        var service = new PronunciationAssessmentService(_config, _logger);

        // Act
        IReadOnlyList<WordPronunciationAssessment> result =
            await service.GetSpeechAssessmentAsync(
                ConvertOggToPcm(inputData), Language.English, default);

        // Assert
        result.Count.Should().BeGreaterThan(3);
    }

    private byte[] ConvertOggToPcm(byte[] oggData)
    {
        using var oggStream = new MemoryStream(oggData);
        using var vorbis = new VorbisWaveReader(oggStream);
        var resampler = new WdlResamplingSampleProvider(vorbis, 16000);

        // Converts from float to 16-bit PCM
        var to16Bit = new WaveFloatTo16Provider(resampler.ToWaveProvider());

        // Converts to mono
        var mono = new StereoToMonoProvider16(to16Bit) { LeftVolume = 0.5f, RightVolume = 0.5f };

        // Create a new MemoryStream to store the converted PCM data
        using var pcmStream = new MemoryStream();

        // Write the converted PCM data to the MemoryStream
        using (var pcmWriter = new WaveFileWriter(pcmStream, mono.WaveFormat))
        {
            byte[] buffer = new byte[1024];
            int bytesRead;
            while ((bytesRead = mono.Read(buffer, 0, buffer.Length)) > 0)
            {
                pcmWriter.Write(buffer, 0, bytesRead);
            }
        }

        return pcmStream.ToArray();
    }
}
