using BuddyLanguage.Domain.Enumerations;
using BuddyLanguage.Domain.Exceptions.TTS;
using BuddyLanguage.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenAI.ChatGpt.AspNetCore.Models;
using OpenAI.GeneratedKiotaClient;
using OpenAI.GeneratedKiotaClient.Models;

namespace BuddyLanguage.KiotaClient
{
    /// <summary>
    /// Implementation of the ITextToSpeech interface using the OpenAI TTS API.
    /// </summary>
    public class OpenAITextToSpeech : ITextToSpeech
    {
        private readonly OpenAICredentials _openAiCredentials;
        private readonly ILogger<OpenAITextToSpeech> _logger;
        private readonly GeneratedOpenAiClient _client;

        /// <summary>
        /// Initializes a new instance of the <see cref="OpenAITextToSpeech"/> class.
        /// </summary>
        /// <param name="options">Options for OpenAI credentials.</param>
        /// <param name="logger">Logger for logging messages.</param>
        public OpenAITextToSpeech(IOptions<OpenAICredentials> options, ILogger<OpenAITextToSpeech> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _openAiCredentials = options.Value ?? throw new ArgumentNullException(nameof(options));

            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = _openAiCredentials.GetAuthHeader();
            _client = GeneratedClientsFactory.CreateGeneratedOpenAiClient(httpClient);
        }

        /// <summary>
        /// Converts text to an MP3 audio byte array using the OpenAI TTS API.
        /// </summary>
        /// <param name="text">Text to be converted to speech.</param>
        /// <param name="language">Language of the speech.</param>
        /// <param name="voice">Voice gender for the speech.</param>
        /// <param name="speed">Speed of the speech.</param>
        /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
        /// <returns>MP3 audio byte array.</returns>
        public async Task<byte[]> TextToByteArrayAsync(string text, Language language, Voice voice, TtsSpeed speed, CancellationToken cancellationToken)
        {
            // Create an instance of CreateSpeechRequest
            var createSpeechRequest = new CreateSpeechRequest()
            {
                Input = text,
                Model = new CreateSpeechRequest.CreateSpeechRequest_model
                {
                    String = "tts-1"
                },
                ResponseFormat = CreateSpeechRequest_response_format.Mp3,
                Speed = GetSpeakingRate(speed),
                Voice = GetVoiceGender(voice)
            };

            // Send a POST request to the Open AI TTS API
            var responseStream = await _client.Audio.Speech.PostAsync(createSpeechRequest);

            if (responseStream is not null)
            {
                // Log the response
                _logger.LogInformation("OpenAI TTS Response Received.");
                return ReadStream(responseStream); // MP3
            }
            else if (responseStream is null)
            {
                throw new SpeechSynthesizingException("OpenAI Error Audio Stream is null.");
            }
            else
            {
                throw new SpeechSynthesizingException("Unknown OpenAI Error, Please try again later.");
            }
        }

        /// <summary>
        /// Gets the speaking rate based on the specified TtsSpeed.
        /// </summary>
        /// <param name="speed">TtsSpeed enumeration.</param>
        /// <returns>Speaking rate as a double.</returns>
        private double GetSpeakingRate(TtsSpeed speed)
        {
            return speed switch
            {
                TtsSpeed.Xslow => 0.25,
                TtsSpeed.Slow => 0.50,
                TtsSpeed.Medium => 1.0,
                TtsSpeed.Fast => 1.5,
                TtsSpeed.Xfast => 2.0,
                _ => throw new NotSupportedException("The provided TtsSpeed is not supported.")
            };
        }

        /// <summary>
        /// Gets the voice gender based on the specified Voice.
        /// </summary>
        /// <param name="voice">Voice enumeration.</param>
        /// <returns>CreateSpeechRequest_voice representing the voice gender.</returns>
        private CreateSpeechRequest_voice GetVoiceGender(Voice voice)
        {
            return voice switch
            {
                Voice.Male => CreateSpeechRequest_voice.Echo,
                Voice.Female => CreateSpeechRequest_voice.Shimmer,
                _ => throw new NotSupportedException("The provided Voice is not supported.")
            };
        }

        /// <summary>
        /// Reads a stream and converts it to a byte array.
        /// </summary>
        /// <param name="input">Input stream to be read.</param>
        /// <returns>Byte array representing the content of the stream.</returns>
        private byte[] ReadStream(Stream input)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                input.CopyTo(ms);
                return ms.ToArray();
            }
        }
    }
}
