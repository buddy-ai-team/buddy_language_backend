using BuddyLanguage.Domain.Enumerations;
using BuddyLanguage.TextToSpeech;
using Microsoft.CognitiveServices.Speech;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace BuddyLanguage.Infrastructure.IntegrationTest
{
    /// <summary>
    /// Integration tests for the Azure Text-to-Speech service covering various languages and voices.
    /// </summary>
    public class TextToSpeechTests
    {
        private readonly Microsoft.Extensions.Configuration.IConfiguration _config;

        public TextToSpeechTests(Microsoft.Extensions.Configuration.IConfiguration config)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
        }

        /// <summary>
        /// Generates all possible combinations of languages and voices for testing.
        /// </summary>
        public static IEnumerable<(Language, Voice)> GetLanguageVoiceCombinations()
        {
            var combinations = new List<(Language, Voice)>();

            foreach (Language language in Enum.GetValues(typeof(Language)))
            {
                foreach (Voice voice in Enum.GetValues(typeof(Voice)))
                {
                    combinations.Add((language, voice));
                }
            }

            return combinations;
        }

        /// <summary>
        /// Tests whether the Azure Text-to-Speech service correctly synthesizes speech for various languages and voices.
        /// </summary>
        [Fact]
        public async Task All_Azure_TTS_Languages_And_Voices_Synthesize()
        {
            var combinations = GetLanguageVoiceCombinations();

            foreach (var combination in combinations)
            {
                var (language, voice) = combination;

                // Arrange
                var logger = new LoggerFactory().CreateLogger<AzureTextToSpeech>();
                var mockOptions = new Mock<IOptionsSnapshot<AzureTTSConfig>>();
                var textToSpeechClient = new AzureTextToSpeech(mockOptions.Object, logger);
                var text = "Hello"; // You can use any sample text.
                var cancellationToken = CancellationToken.None;

                // Act
                var audioData = await textToSpeechClient.TextToWavByteArrayAsync(text, language, voice, cancellationToken);

                // Assert
                Assert.NotEmpty(audioData);
                Assert.NotNull(audioData);
            }
        }

        /// <summary>
        /// Verifies the presence and validity of Azure API keys for Text-to-Speech synthesis.
        /// </summary>
        [Fact]
        public void Verify_Azure_Api_Keys()
        {
            // Arrange
            string? speechKey = _config.GetValue<string>("AzureTTSConfig:ASPNETCORE_AZURE_SPEECH_KEY");
            string? speechRegion = _config.GetValue<string>("AzureTTSConfig:ASPNETCORE_AZURE_SPEECH_REGION"); 

            // Act
            ValidateAzureApiKeys(speechKey, speechRegion);

            // Additional assertions for synthesizer or other validation if needed
            var speechConfig = SpeechConfig.FromSubscription(speechKey, speechRegion);
            var synthesizer = new SpeechSynthesizer(speechConfig);
            Assert.NotNull(synthesizer); // Synthesizer will be null or throw an error if any of the keys are null
        }

        // Helper method to validate Azure API keys
        private void ValidateAzureApiKeys(string? speechKey, string? speechRegion)
        {
            // Assert
            AssertApiKeyNotNull(speechKey, "Speech Key");
            AssertApiKeyNotNull(speechRegion, "Speech Region");
        }

        // Helper method to assert that an API key is not null
        private void AssertApiKeyNotNull(string? apiKey, string apiKeyName)
        {
            Assert.True(apiKey != null, $"{apiKeyName} is null. Please set the environment variable for {apiKeyName}.");
        }
    }
}
