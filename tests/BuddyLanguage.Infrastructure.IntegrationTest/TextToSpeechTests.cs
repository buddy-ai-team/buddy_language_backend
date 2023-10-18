using BuddyLanguage.Domain.Enumerations;
using BuddyLanguage.TextToSpeech;
using FluentAssertions;
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
        public async Task All_azure_TTS_languages_and_voices_synthesized()
        {
            var combinations = GetLanguageVoiceCombinations();

            foreach (var combination in combinations)
            {
                var (language, voice) = combination;

                // Arrange
                var logger = new LoggerFactory().CreateLogger<AzureTextToSpeech>();
                var options = Options.Create(new AzureTTSConfig()
                    {
                        SpeechKey = GetKeyFromEnvironment("AZURE_SPEECH_KEY"),
                        SpeechRegion = GetKeyFromEnvironment("AZURE_SPEECH_REGION")
                    }
                );

                var textToSpeechClient = new AzureTextToSpeech(options, logger);
                var text = "Hello"; // You can use any sample text.
                var cancellationToken = CancellationToken.None;

                // Act
                var audioData = await textToSpeechClient.TextToWavByteArrayAsync(text, language, voice, cancellationToken);

                // Assert
                audioData.Should().NotBeNullOrEmpty($"Audio Data For Language: {language} and Voice: {voice} combination was null/empty!");
            }
        }

        /// <summary>
        /// Verifies the presence and validity of Azure API keys for Text-to-Speech synthesis.
        /// </summary>
        [Fact]
        public void Azure_config_is_valid()
        {
            // Arrange
            //Act
            string? speechKey = GetKeyFromEnvironment("AZURE_SPEECH_KEY");
            string? speechRegion = GetKeyFromEnvironment("AZURE_SPEECH_REGION");

            // Assert
            var speechConfig = SpeechConfig.FromSubscription(speechKey, speechRegion);
            var synthesizer = new SpeechSynthesizer(speechConfig);
            synthesizer.Should().NotBeNull("Synthesiser Was Null! Please double check key validity and that they are defined!"); // Synthesizer will be null or throw an error if any of the keys are null
        }

        //Helper Method To Validate Env Variables
        private string GetKeyFromEnvironment(string keyName)
        {
            if (keyName == null) throw new ArgumentNullException(nameof(keyName));
            var value = Environment.GetEnvironmentVariable(keyName);
            if (value is null)
            {
                throw new InvalidOperationException($"{keyName} is not set as environment variable");
            }

            return value;
        }
    }
}
