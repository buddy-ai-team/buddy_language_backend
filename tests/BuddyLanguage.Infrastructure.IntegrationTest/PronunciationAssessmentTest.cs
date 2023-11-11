using BuddyLanguage.AzureServices;
using BuddyLanguage.Domain.Entities;
using FluentAssertions;
using Microsoft.CognitiveServices.Speech;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
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

        // var reader = new WaveFileReader("assets/Pronunciation.wav");
        // var mono1 = new StereoToMonoProvider16(mono);

        // Downsample to 8000 filter.
        var resamplingProvider = new WdlResamplingSampleProvider(mono, 8000);

        var waveProvider = new SampleToWaveProvider16(resamplingProvider);

        var outputDevice = new WaveOutEvent();
        outputDevice.Init(waveProvider);
        outputDevice.Play();
        while (outputDevice.PlaybackState == PlaybackState.Playing)
        {
            Thread.Sleep(1000);
        }

        var outputStream = new MemoryStream();
        var waveFileWriter = new WaveFileWriter(outputStream, waveProvider.WaveFormat);
        byte[] buffer = new byte[waveProvider.WaveFormat.AverageBytesPerSecond];
        int read;
        while ((read = waveProvider.Read(buffer, 0, buffer.Length)) > 0)
        {
#pragma warning disable VSTHRD103
            waveFileWriter.Write(buffer, 0, read);
#pragma warning restore VSTHRD103
        }

        // await waveFileWriter.FlushAsync();
        var service = new PronunciationAssessmentService(_config, _logger);

        //Act
        IReadOnlyList<WordPronunciationAssessment> result =
            await service.GetSpeechAssessmentAsync(outputStream.GetBuffer(), default);

        // Assert
        result.Count.Should().NotBe(0);
    }
}
