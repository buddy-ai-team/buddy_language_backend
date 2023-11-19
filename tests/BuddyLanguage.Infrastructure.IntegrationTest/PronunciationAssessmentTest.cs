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

        var sampleWriter = new SampleToWaveProvider16(resamplingProvider);
        var waveProvider16 = sampleWriter.ToSampleProvider().ToWaveProvider16();

        var aaa = WaveProviderToBytes(waveProvider16);

        var service = new PronunciationAssessmentService(_config, _logger);

        //Act
        IReadOnlyList<WordPronunciationAssessment> result =
            await service.GetSpeechAssessmentAsync(aaa, default);

        // Assert
        result.Count.Should().NotBe(0);
    }

    private byte[] WaveProviderToBytes(IWaveProvider waveProvider)
    {
        // Create a SampleProvider from the WaveProvider
        ISampleProvider sampleProvider = waveProvider.ToSampleProvider();

        // Specify the number of samples to read (adjust as needed)
        int bufferSize = 1024;

        // Create a byte array to store the raw PCM data
        byte[] pcmData = new byte[bufferSize * 2]; // Assuming 16-bit PCM, adjust as needed

        // Create a MemoryStream to store the raw PCM data
        using (MemoryStream pcmStream = new MemoryStream())
        {
            int samplesRead;

            // Read raw PCM data from the SampleProvider into the MemoryStream
            while ((samplesRead = sampleProvider.Read(pcmData, 0, bufferSize)) > 0)
            {
                pcmStream.Write(pcmData, 0, samplesRead * 2); // Assuming 16-bit PCM, adjust as needed
            }

            // Retrieve the raw PCM data as a byte array
            byte[] rawPcmBytes = pcmStream.ToArray();

            // Now 'rawPcmBytes' contains the raw PCM data without any headers
            Console.WriteLine("Conversion completed.");
        }
    }
}
