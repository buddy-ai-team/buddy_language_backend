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

    [Fact]
    public async Task Result_of_assessment_calculated()
    {
        byte[] data = ConvertToMono16BitPcm("assets/Pronunciation.wav");
        var service = new PronunciationAssessmentService(_config, _logger);

        //Act
        IReadOnlyList<WordPronunciationAssessment> result =
            await service.GetSpeechAssessmentAsync(data, default);

        // Assert
        result.Count.Should().NotBe(0);
    }

    private byte[] ConvertToMono16BitPcm(string fileName)
    {
        // Read the WAV file into a byte array
        byte[] wavData = File.ReadAllBytes(fileName);
        using (MemoryStream inputStream = new MemoryStream(wavData))
        {
            using (WaveStream reader = new WaveFileReader(inputStream))
            {
                // Convert to 16-bit PCM
                var pcmFormat = new WaveFormat(16000, 16, 1); // Adjust sample rate as needed
                var pcmStream = new WaveFormatConversionStream(pcmFormat, reader);

                // Convert to byte array
                using (MemoryStream outputStream = new MemoryStream())
                {
                    pcmStream.CopyTo(outputStream);
                    return outputStream.ToArray();
                }
            }
        }
    }

    private byte[] ConvertToByteArray(ISampleProvider sampleProvider)
    {
        int sampleRate = sampleProvider.WaveFormat.SampleRate;
        int channels = sampleProvider.WaveFormat.Channels;

        // Determine a reasonable buffer size (you may need to adjust this based on your needs)
        int bufferSize = sampleRate * channels; // 1 second of audio data

        // Create a byte array to hold the converted samples
        byte[] buffer = new byte[bufferSize * sizeof(float)];

        WaveBuffer waveBuffer = new WaveBuffer(buffer);

        int bytesRead;
        MemoryStream stream = new MemoryStream();

        do
        {
            bytesRead = sampleProvider.Read(waveBuffer, 0, bufferSize);
            stream.Write(buffer, 0, bytesRead * sizeof(float));
        }
        while (bytesRead > 0);

        return stream.ToArray();
    }
}
