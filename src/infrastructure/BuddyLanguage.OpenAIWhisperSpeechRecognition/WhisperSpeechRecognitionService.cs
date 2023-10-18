using OpenAI.ObjectModels.RequestModels;
using OpenAI.ObjectModels;
using BuddyLanguage.Domain.Interfaces;
using BuddyLanguage.Domain.Exceptions;
using Microsoft.Extensions.DependencyInjection;
using OpenAI.Interfaces;

namespace BuddyLanguage.OpenAIWhisperSpeechRecognitionService
{
    public class WhisperSpeechRecognitionService : ISpeechRecognitionService
    {
        private readonly IOpenAIService _openAIService;
        public WhisperSpeechRecognitionService(IServiceProvider serviceProvider)
        {
            _openAIService = serviceProvider.GetRequiredService<IOpenAIService>();      
        }
        public async Task<string> RecognizeSpeechToTextAsync
            (byte[] voiceMessage, CancellationToken cancellationToken)
        {
            if (voiceMessage.Length <= 0)
            {
                throw new ArgumentException(nameof(voiceMessage));
            }

              var response = await _openAIService
                .Audio
                .CreateTranscription(new AudioCreateTranscriptionRequest()
                {
                    FileName = "VoiceMessage",
                    File = voiceMessage,
                    Model = Models.WhisperV1,
                    ResponseFormat = StaticValues.AudioStatics.ResponseFormat.VerboseJson
                }, cancellationToken);

            if (!response.Successful)
            {
                throw new InvalidSpeechRecognitionException("Can`t recognize speech to text!");
            }

            var textMessage = string.Join("\n", response.Text);
            return textMessage;
        }
    }
}