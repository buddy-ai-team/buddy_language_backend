using BuddyLanguage.Domain.Enumerations;
using BuddyLanguage.Domain.Exceptions.TTS;
using Microsoft.Extensions.Logging;
using OpenAI.GeneratedKiotaClient;
using OpenAI.GeneratedKiotaClient.Audio.Speech;
using OpenAI.GeneratedKiotaClient.Models;

namespace BuddyLanguage.KiotaClient
{
    public class OpenAITextToSpeech
    {
        private readonly GeneratedOpenAiClient _client;
        private readonly ILogger<OpenAITextToSpeech> _logger;

        public OpenAITextToSpeech(ILogger<OpenAITextToSpeech> logger)
        {
            _logger = logger;
            _client = GeneratedClientsFactory.CreateGeneratedOpenAiClient(new HttpClient());
        }

        public async Task<byte[]> TextToMP3ByteArrayAsync(string text, Language language, Voice voice, TtsSpeed speed, CancellationToken cancellationToken)
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

            // Create an instance of SpeechRequestBuilder
            var speechRequestBuilder = new SpeechRequestBuilder(new Dictionary<string, object>(), _client.RequestAdapter);

            // Send a POST request to the Open AI TTS API
            var responseStream = await speechRequestBuilder.PostAsync(createSpeechRequest);

            if (responseStream is not null)
            {
                // Log the response
                _logger.LogInformation("OpenAI TTS Response Received.");
                return ReadStream(responseStream);
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

        private CreateSpeechRequest_voice GetVoiceGender(Voice voice)
        {
            return voice switch
            {
                Voice.Male => CreateSpeechRequest_voice.Echo,
                Voice.Female => CreateSpeechRequest_voice.Shimmer,
                _ => throw new NotSupportedException("The provided Voice is not supported.")
            };
        }

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
