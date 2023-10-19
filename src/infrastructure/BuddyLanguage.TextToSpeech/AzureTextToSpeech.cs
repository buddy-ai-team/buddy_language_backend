﻿using BuddyLanguage.Domain.Enumerations;
using BuddyLanguage.Domain.Interfaces;
using Microsoft.CognitiveServices.Speech;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BuddyLanguage.TextToSpeech
{

    /// <summary>
    /// Implementation of the ITextToSpeech interface using Microsoft Azure Cognitive Services Text-to-Speech.
    /// </summary>
    public class AzureTextToSpeech : ITextToSpeech
    {

        private readonly ILogger<AzureTextToSpeech> _logger;
        private readonly AzureTTSConfig _config;

        /// <summary>
        /// Initializes a new instance of the AzureTextToSpeech class.
        /// </summary>
        /// <param name="logger">The logger for logging messages.</param>
        public AzureTextToSpeech(IOptions<AzureTTSConfig> config, ILogger<AzureTextToSpeech> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _config = config.Value ?? throw new ArgumentNullException(nameof(config));
        }

        /// <summary>
        /// Converts text to a WAV audio byte array using Azure Cognitive Services Text-to-Speech.
        /// </summary>
        /// <param name="text">The text to be synthesized into speech.</param>
        /// <param name="language">The language of the voice.</param>
        /// <param name="voice">The desired voice for synthesis.</param>
        /// <param name="cancellationToken">A CancellationToken for possible cancellation of the operation.</param>
        /// <returns>A byte array containing the synthesized audio.</returns>
        public async Task<byte[]> TextToWavByteArrayAsync(string text, Language language, Voice voice, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(text);
            ArgumentNullException.ThrowIfNull(language);
            ArgumentNullException.ThrowIfNull(voice);

            var speechKey = _config.SpeechKey;
            var speechRegion = _config.SpeechRegion;

            var speechConfig = SpeechConfig.FromSubscription(speechKey, speechRegion);
            speechConfig.SpeechSynthesisVoiceName = GetSynthesisVoiceNameFromEnum(language, voice);

            using (var synthesizer = new SpeechSynthesizer(speechConfig))
            {
                using (var resultTask = await synthesizer.SpeakTextAsync(text).WaitAsync(cancellationToken))
                {
                    if (resultTask.Reason == ResultReason.SynthesizingAudioCompleted)
                    {
                        _logger.LogInformation("Speech synthesized to byte array.");
                        return resultTask.AudioData;
                    }
                    else if (resultTask.Reason == ResultReason.Canceled)
                    {
                        var cancellation = SpeechSynthesisCancellationDetails.FromResult(resultTask);
                        _logger.LogInformation($"Speech synthesis canceled. Reason: {cancellation.Reason}");

                        if (cancellation.Reason == CancellationReason.Error)
                        {
                            _logger.LogInformation($"Speech synthesis error. ErrorCode={cancellation.ErrorCode}, ErrorDetails={cancellation.ErrorDetails}");
                        }
                    }
                }
            }

            //#pragma warning disable CS0161 because it classifies it as an Error
            return null;
        }

        /// <summary>
        /// Retrieves the Azure Cognitive Services Text-to-Speech voice name based on the language and desired voice type.
        /// </summary>
        /// <param name="language">The language of the voice.</param>
        /// <param name="voice">The desired voice for synthesis.</param>
        /// <returns>The Azure Cognitive Services voice name corresponding to the given language and voice type.</returns>
        public string GetSynthesisVoiceNameFromEnum(Language language, Voice voice)
        {
            ArgumentNullException.ThrowIfNull(language);
            ArgumentNullException.ThrowIfNull(voice);

            string voiceName = (language, voice) switch
            {
                (Language.Russian, Voice.Female) => "ru-RU-SvetlanaNeural",
                (Language.Russian, Voice.Male) => "ru-RU-DmitryNeural",
                (Language.English, Voice.Female) => "en-US-JennyNeural",
                (Language.English, Voice.Male) => "en-US-GuyNeural",
                _ => throw new NotSupportedException("The Language/Voice You Provided Is Not Currently Supported By Our Project!")
            };

            return voiceName;
        }
    }
}