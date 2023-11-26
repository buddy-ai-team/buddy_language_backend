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
        // byte[] inputData = System.IO.File.ReadAllBytes("assets/Pronunciation.wav");
        byte[] inputData = Convert("assets/Pronunciation.wav");

        // byte[] data = await File.ReadAllBytesAsync("assets/Pronunciation.wav");
        var service = new PronunciationAssessmentService(_config, _logger);

        //Act
        IReadOnlyList<WordPronunciationAssessment> result =
            await service.PronunciationAssessmentWithStreamInternalAsync(HeaderCutter(inputData), default);

        // Assert
        result.Count.Should().NotBe(0);
    }

    private byte[] GetData()
    {
        var audioDataWithHeader = File.ReadAllBytes("assets/Sample.wav");
        var audioData = new byte[audioDataWithHeader.Length - 46];
        Array.Copy(audioDataWithHeader, 46, audioData, 0, audioData.Length);
        return audioData;
    }

    private byte[] Convert(string inputFile)
    {
        // int outRate = 16000;
        using (var reader = new AudioFileReader(inputFile))
        {
            // var resampler = new WdlResamplingSampleProvider(reader, outRate);
            var pcmData = new WaveFloatTo16Provider(reader.ToWaveProvider());

            WaveFileWriter.CreateWaveFile16("assets/Sample.wav", pcmData.ToSampleProvider());
            using (var memoryStream = new MemoryStream())
            {
                WaveFileWriter.WriteWavFileToStream(memoryStream, pcmData);
                return memoryStream.ToArray();
            }
        }
    }

    private byte[] HeaderCutter(byte[] audioDataWithHeader)
    {
        var audioData = new byte[audioDataWithHeader.Length - 46];
        Array.Copy(audioDataWithHeader, 46, audioData, 0, audioData.Length);
        return audioData;
    }
}
