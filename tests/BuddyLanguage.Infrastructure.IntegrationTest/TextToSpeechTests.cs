using BuddyLanguage.Domain.Entities;
using BuddyLanguage.TextToSpeech;
using Microsoft.CognitiveServices.Speech;
using Microsoft.Extensions.Logging;

namespace BuddyLanguage.Infrastructure.IntegrationTest;

public class TextToSpeechTests
{
    [Fact]
    public async Task TextToMP3FileAsync_SavesTextToMp3File()
    {
        // Arrange
        var logger = new LoggerFactory().CreateLogger<AzureTextToSpeech>();
        var textToSpeechClient = new AzureTextToSpeech(logger);
        var text = "Привет, мир!";
        var outputFilePath = "output.mp3"; // Provide a valid path where you want to save the MP3 file.
        var voice = SynthesisVoices.RussianFemale;

        try
        {
            // Act
            await textToSpeechClient.TextToMP3FileAsync(text, outputFilePath, voice);

            // Assert
            // Check if the WAV file was created and the outputFilePath exists.
            Assert.True(File.Exists(outputFilePath));

            // Clean up the test by deleting the created MP3 file.
            File.Delete(outputFilePath);
        }
        catch (Exception ex)
        {
            Assert.True(false, $"Test failed: {ex.Message}");
        }
    }

    [Fact]
    public void VerifyAzureApiKeys()
    {
        string speechKey = Environment.GetEnvironmentVariable("AZURE_SPEECH_KEY");
        string speechRegion = Environment.GetEnvironmentVariable("AZURE_SPEECH_REGION");

        Assert.NotNull(speechKey);
        Assert.NotNull(speechRegion);

        try
        {
            var speechConfig = SpeechConfig.FromSubscription(speechKey, speechRegion);
            var synthesizer = new SpeechSynthesizer(speechConfig);

            Assert.NotNull(synthesizer);
        }
        catch
        {
            Assert.True(false, "API key validation failed");
        }
    }
}
