using OpenAI.Managers;
using OpenAI.ObjectModels.RequestModels;
using OpenAI.ObjectModels;
using BuddyLanguage.Domain.Interfaces;
using BuddyLanguage.Domain.Exceptions;

namespace BuddyLanguage.OpenAIWhisperSpeechRecognition
{
    public class WhisperSpeechRecognizer : ISpeechRecognizer
    {
        private readonly OpenAIService _openAIService;
        public WhisperSpeechRecognizer(OpenAIService openAIService)
        {
            _openAIService = openAIService ?? throw new ArgumentException(nameof(openAIService));    
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
                throw new InvalidSpeechRecognitionException("Can`t recognixe speech to text!"); 
            }

            var textMessage = string.Join("\n", response.Text);
            return textMessage; 
        }
    }
}