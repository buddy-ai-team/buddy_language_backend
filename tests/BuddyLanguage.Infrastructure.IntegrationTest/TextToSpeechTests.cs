﻿using BuddyLanguage.AzureServices;
using BuddyLanguage.Domain.Enumerations;
using BuddyLanguage.KiotaClient;
using FluentAssertions;
using Microsoft.CognitiveServices.Speech;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenAI.ChatGpt.AspNetCore.Models;

namespace BuddyLanguage.Infrastructure.IntegrationTest
{
    /// <summary>
    /// Integration tests for the Azure Text-to-Speech service covering various languages and voices.
    /// </summary>
    public class TextToSpeechTests
    {
        /// <summary>
        /// Tests whether the Azure Text-to-Speech service correctly synthesizes speech for various languages and voices and speeds.
        /// </summary>
        /// <returns>Nothing</returns>
        [Fact]
        public async Task All_azure_TTS_languages_voices_and_speeds_synthesized()
        {
            // Arrange
            var logger = new LoggerFactory().CreateLogger<AzureTextToSpeech>();
            var options = Options.Create(new AzureConfig()
            {
                SpeechKey = GetKeyFromEnvironment("AZURE_SPEECH_KEY"),
                SpeechRegion = GetKeyFromEnvironment("AZURE_SPEECH_REGION")
            });

            var textToSpeechClient = new AzureTextToSpeech(options, logger);
            var text = "Hi"; // You can use any sample text.
            var cancellationToken = CancellationToken.None;

            Language[] languages = { Language.English, Language.Russian };
            foreach (Language language in languages)
            {
                // Act
                var audioData = await textToSpeechClient.TextToByteArrayAsync(text, language, Voice.Male, TtsSpeed.Medium, cancellationToken);

                // Assert
                audioData.Should().NotBeNullOrEmpty($"Audio Data For Language: {language} was null/empty!");
            }

            foreach (Voice voice in Enum.GetValues(typeof(Voice)))
            {
                // Act
                var audioData = await textToSpeechClient.TextToByteArrayAsync(text, Language.English, voice, TtsSpeed.Medium, cancellationToken);

                // Assert
                audioData.Should().NotBeNullOrEmpty($"Audio Data For Voice: {voice} was null/empty!");
            }

            foreach (TtsSpeed speed in Enum.GetValues(typeof(TtsSpeed)))
            {
                // Act
                var audioData = await textToSpeechClient.TextToByteArrayAsync(text, Language.English, Voice.Male, speed, cancellationToken);

                // Assert
                audioData.Should().NotBeNullOrEmpty($"Audio Data For Voice: {speed} was null/empty!");
            }
        }

        /// <summary>
        /// Tests whether the OpenAI Text-to-Speech service correctly synthesizes speech for various languages and voices and speeds.
        /// </summary>
        /// <returns>Nothing</returns>
        [Fact]
        public async Task All_openAI_TTS_languages_voices_and_speeds_synthesized()
        {
            // Arrange
            var logger = new LoggerFactory().CreateLogger<OpenAITextToSpeech>();
            var options = Options.Create(new OpenAICredentials()
            {
                ApiKey = GetKeyFromEnvironment("OPENAI_API_KEY"),
                ApiHost = "https://api.openai.com/v1/"
            });

            var textToSpeechClient = new OpenAITextToSpeech(new HttpClient(), options, logger);
            var text = "Hi"; // You can use any sample text.
            var cancellationToken = CancellationToken.None;

            foreach (Voice voice in Enum.GetValues(typeof(Voice)))
            {
                // Act
                var audioData = await textToSpeechClient.TextToByteArrayAsync(text, Language.English, voice, TtsSpeed.Medium, cancellationToken);

                // Assert
                audioData.Should().NotBeNullOrEmpty($"Audio Data For Voice: {voice} was null/empty!");
            }

            foreach (TtsSpeed speed in Enum.GetValues(typeof(TtsSpeed)))
            {
                // Act
                var audioData = await textToSpeechClient.TextToByteArrayAsync(text, Language.English, Voice.Male, speed, cancellationToken);

                // Assert
                audioData.Should().NotBeNullOrEmpty($"Audio Data For Voice: {speed} was null/empty!");
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
            string speechKey = GetKeyFromEnvironment("AZURE_SPEECH_KEY");
            string speechRegion = GetKeyFromEnvironment("AZURE_SPEECH_REGION");

            // Assert
            var speechConfig = SpeechConfig.FromSubscription(speechKey, speechRegion);
            var synthesizer = new SpeechSynthesizer(speechConfig);
            synthesizer.Should().NotBeNull("Synthesiser Was Null! Please double check key validity and that they are defined!"); // Synthesizer will be null or throw an error if any of the keys are null
        }

        //Helper Method To Validate Env Variables
        private string GetKeyFromEnvironment(string keyName)
        {
            if (keyName == null)
            {
                throw new ArgumentNullException(nameof(keyName));
            }

            var value = Environment.GetEnvironmentVariable(keyName);
            if (value is null)
            {
                throw new InvalidOperationException($"{keyName} is not set as environment variable");
            }

            return value;
        }
    }
}
