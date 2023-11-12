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

    public static byte[] ToArray(Stream stream)
    {
        byte[] buffer = new byte[4096];
        int reader = 0;
        MemoryStream memoryStream = new MemoryStream();
        while ((reader = stream.Read(buffer, 0, buffer.Length)) != 0)
        {
            memoryStream.Write(buffer, 0, reader);
        }

        return memoryStream.ToArray();
    }

    [Fact]
    public async Task Result_of_assessment_calculated()
    {
        var inputReader = new AudioFileReader("assets/Pronunciation.wav");
        var sampleStream = new WaveToSampleProvider(inputReader);
        var mono = new StereoToMonoSampleProvider(sampleStream)
            {
                LeftVolume = 0.0f,
                RightVolume = 1.0f
            };

        // Downsample to 8000 filter.
        var resamplingProvider = new WdlResamplingSampleProvider(mono, 8000);

        var waveProvider = new SampleToWaveProvider16(resamplingProvider);

        // player (just to test)
        // var outputDevice = new WaveOutEvent();
        // outputDevice.Init(waveProvider);
        // outputDevice.Play();
        // while (outputDevice.PlaybackState == PlaybackState.Playing)
        // {
        //     Thread.Sleep(1000);
        // }
        // end of play test
        var aaa = ConvertWavToByteArray("assets/Pronunciation.wav");

        // await waveFileWriter.FlushAsync();
        var service = new PronunciationAssessmentService(_config, _logger);

        //Act
        IReadOnlyList<WordPronunciationAssessment> result =
            await service.GetSpeechAssessmentAsync(aaa, default);

        // Assert
        result.Count.Should().NotBe(0);
    }

    public byte[] ConvertWavToByteArray(string fileName)
    {
        using WaveFileReader reader = new WaveFileReader(fileName);
        Assert.Equal(16, reader.WaveFormat.BitsPerSample);
        byte[] buffer = new byte[reader.Length];
        int read = reader.Read(buffer, 0, buffer.Length);
        short[] sampleBuffer = new short[read / 2];
        Buffer.BlockCopy(buffer, 0, sampleBuffer, 0, read);
        return buffer;
    }
}
