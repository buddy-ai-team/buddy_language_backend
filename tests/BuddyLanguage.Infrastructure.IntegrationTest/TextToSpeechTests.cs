using BuddyLanguage.Domain.Enumerations;
using BuddyLanguage.TextToSpeech;
using Microsoft.CognitiveServices.Speech;
using Microsoft.Extensions.Logging;

namespace BuddyLanguage.Infrastructure.IntegrationTest
{
    /// <summary>
    /// Integration tests for the Azure Text-to-Speech service covering various languages and voices.
    /// </summary>
    public class TextToSpeechTests
    {
        /// <summary>
        /// Generates all possible combinations of languages and voices for testing.
        /// </summary>
        public static IEnumerable<object[]> GetLanguageVoiceCombinations()
        {
            foreach (Language language in Enum.GetValues(typeof(Language)))
            {
                foreach (Voice voice in Enum.GetValues(typeof(Voice)))
                {
                    yield return new object[] { language, voice };
                }
            }
        }

        /// <summary>
        /// Tests whether the Azure Text-to-Speech service correctly synthesizes speech for various languages and voices.
        /// </summary>
        [Theory]
        [MemberData(nameof(GetLanguageVoiceCombinations))]
        public async Task All_Azure_TTS_Languages_And_Voices_Synthesize(Language language, Voice voice)
        {
            // Arrange
            var logger = new LoggerFactory().CreateLogger<AzureTextToSpeech>();
            var textToSpeechClient = new AzureTextToSpeech(logger);
            var text = "Hello"; // You can use any sample text.
            var cancellationToken = CancellationToken.None;

            try
            {
                // Act
                var audioData = await textToSpeechClient.TextToWavByteArrayAsync(text, language, voice, cancellationToken);

                // Assert
                Assert.NotNull(audioData);
            }
            catch (Exception ex)
            {
                Assert.True(false, $"Test failed for language {language} and voice {voice}: {ex.Message}");
            }
        }

        /// <summary>
        /// Verifies the presence and validity of Azure API keys for Text-to-Speech synthesis.
        /// </summary>
        [Fact]
        public void Verify_Azure_Api_Keys()
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
}
