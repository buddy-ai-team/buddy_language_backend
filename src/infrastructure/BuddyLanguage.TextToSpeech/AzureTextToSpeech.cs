using BuddyLanguage.Domain.Enumerations;
using BuddyLanguage.Domain.Exceptions.TTS;
using BuddyLanguage.Domain.Interfaces;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using static BuddyLanguage.Domain.Enumerations.Language;
using static BuddyLanguage.Domain.Enumerations.Voice;

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
        /// Initializes a new instance of the <see cref="AzureTextToSpeech"/> class.
        /// </summary>
        /// <param name="config">Configuration of AzureTTS</param>
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
        /// <param name="speed">The desired voice speed for synthesis.</param>
        /// <param name="cancellationToken">A CancellationToken for possible cancellation of the operation.</param>
        /// <returns>A byte array containing the synthesized audio.</returns>
        public async Task<byte[]> TextToWavByteArrayAsync(string text, Language language, Voice voice, TtsSpeed speed, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(text);

            var speechKey = _config.SpeechKey;
            var speechRegion = _config.SpeechRegion;

            var speechConfig = SpeechConfig.FromSubscription(speechKey, speechRegion);
            speechConfig.SpeechSynthesisVoiceName = GetSynthesisVoiceNameFromEnum(language, voice);

            string voicespeed = GetSsmlSpeakingRate(speed);

            //Make sure text doesn't have any dangerous symbols
            text = SanitizeTextForSSML(text);

            // Create SSML with speaking rate adjustment (adjust the rate value as needed)
            var ssml = $@"<speak version='1.0' xmlns='http://www.w3.org/2001/10/synthesis' xmlns:mstts='http://www.w3.org/2001/mstts' xml:lang='{language.ToString()}'>
                    <voice name='{speechConfig.SpeechSynthesisVoiceName}'>
                        <mstts:express-as type='{voicespeed}'>
                            {text}
                        </mstts:express-as>
                    </voice>
                </speak>";

            using var speechSynthesizer = new SpeechSynthesizer(speechConfig, null);

            var result = await speechSynthesizer.SpeakSsmlAsync(ssml).WaitAsync(cancellationToken);

            switch (result.Reason)
            {
                case ResultReason.SynthesizingAudioCompleted:
                    _logger.LogInformation("Speech synthesized to byte array");
                    return result.AudioData;
                case ResultReason.Canceled:
                    {
                        var cancellation = SpeechSynthesisCancellationDetails.FromResult(result);
                        if (cancellation.Reason == CancellationReason.Error)
                        {
                            _logger.LogError(
                                "Speech synthesis error. ErrorCode={CancellationErrorCode}, ErrorDetails={CancellationErrorDetails}",
                                cancellation.ErrorCode,
                                cancellation.ErrorDetails);
                        }

                        throw new SpeechSynthesizingException($"Speech synthesis failed. " +
                                                            $"Error: {cancellation.ErrorCode} {cancellation.ErrorDetails}");
                    }

                default:
                    throw new SpeechSynthesizingException($"Speech synthesis failed. " +
                                                        $"ResultReason: {result.Reason}");
            }
        }

        /// <summary>
        /// Maps a TtsSpeed enumeration value to its corresponding SSML speaking rate adjustment value.
        /// </summary>
        /// <param name="speed">The TtsSpeed value to map to an SSML speaking rate adjustment value.</param>
        /// <returns>The SSML speaking rate adjustment value corresponding to the provided TtsSpeed.</returns>
        private string GetSsmlSpeakingRate(TtsSpeed speed)
        {
            return speed switch
            {
                TtsSpeed.Xslow => "x-slow",
                TtsSpeed.Slow => "slow",
                TtsSpeed.Medium => "medium",
                TtsSpeed.Fast => "fast",
                TtsSpeed.Xfast => "x-fast",
                _ => throw new NotSupportedException("The provided TtsSpeed is not supported.")
            };
        }

        /// <summary>
        /// Retrieves the Azure Cognitive Services Text-to-Speech voice name based on the language and desired voice type.
        /// </summary>
        /// <param name="language">The language of the voice.</param>
        /// <param name="voice">The desired voice for synthesis.</param>
        /// <returns>The Azure Cognitive Services voice name corresponding to the given language and voice type.</returns>
        private string GetSynthesisVoiceNameFromEnum(Language language, Voice voice)
        {
            return (language, voice) switch
            {
                (Russian, Female) => "ru-RU-SvetlanaNeural",
                (Russian, Male) => "ru-RU-DmitryNeural",
                (English, Female) => "en-US-JennyNeural",
                (English, Male) => "en-US-GuyNeural",
                _ => throw new NotSupportedException("The Language/Voice You Provided Is Not Currently Supported By Our Project!")
            };
        }

        /// <summary>
        /// Replaces special characters in the input text with safe equivalents or their respective XML entities to sanitize it for SSML (Speech Synthesis Markup Language).
        /// </summary>
        /// <param name="text">The input text that needs to be sanitized for SSML.</param>
        /// <returns>The sanitized text ready for use in SSML.</returns>
        private string SanitizeTextForSSML(string text)
        {
            // Replace special characters with safe equivalents or their respective XML entities
            text = text
                .Replace("&", "and")
                .Replace("<", "&lt;")
                .Replace(">", "&gt;")
                .Replace("\"", "&quot;")
                .Replace("'", "&apos;")
                .Replace("!", "&#33;")
                .Replace("#", "&#35;")
                .Replace("$", "&#36;")
                .Replace("%", "&#37;")
                .Replace("(", "&#40;")
                .Replace(")", "&#41;")
                .Replace("*", "&#42;")
                .Replace("+", "&#43;")
                .Replace(",", "&#44;")
                .Replace("/", "&#47;")
                .Replace(":", "&#58;")
                .Replace(";", "&#59;")
                .Replace("=", "&#61;")
                .Replace("?", "&#63;")
                .Replace("[", "&#91;")
                .Replace("\\", "&#92;")
                .Replace("]", "&#93;")
                .Replace("^", "&#94;")
                .Replace("_", "&#95;");

            return text;
        }
    }
}
