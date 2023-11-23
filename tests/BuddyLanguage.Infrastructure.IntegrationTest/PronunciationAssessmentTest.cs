using System;
using System.IO;
using System.IO;
using System.Threading.Tasks;
using BuddyLanguage.AzureServices;
using BuddyLanguage.Domain.Entities;
using FluentAssertions;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NAudio.Utils;
using NAudio.Vorbis;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using NVorbis;

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
        byte[] inputData = System.IO.File.ReadAllBytes("assets/Pronunciation.ogg");
        byte[] data = ConvertOggToPcm(inputData);

        // byte[] data = await File.ReadAllBytesAsync("assets/Pronunciation.wav");
        var service = new PronunciationAssessmentService(_config, _logger);

        //Act
        IReadOnlyList<WordPronunciationAssessment> result =
            await service.GetSpeechAssessmentAsync(data, default);

        // Assert
        result.Count.Should().NotBe(0);
    }

    private byte[] ConvertOggToPcm(byte[] oggData)
    {
        using (var oggStream = new MemoryStream(oggData))
        using (var pcmStream = new MemoryStream())
        {
            using (var readerStream = new VorbisWaveReader(oggStream))
            {
                var waveFormat = new WaveFormat(16000, 16, 1); // 16kHz, 16bit, mono
                using (var resampler = new MediaFoundationResampler(readerStream, waveFormat))
                {
                    WaveFileWriter.WriteWavFileToStream(pcmStream, resampler);
                    return pcmStream.ToArray();
                }
            }
        }
    }
}
