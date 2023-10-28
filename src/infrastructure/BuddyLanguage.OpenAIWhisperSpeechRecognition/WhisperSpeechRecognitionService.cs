using BuddyLanguage.Domain.Exceptions;
using BuddyLanguage.Domain.Interfaces;
using OpenAI.Interfaces;
using OpenAI.ObjectModels;
using OpenAI.ObjectModels.RequestModels;

namespace BuddyLanguage.OpenAIWhisperSpeechRecognitionService
{
    public class WhisperSpeechRecognitionService : ISpeechRecognitionService
    {
        private readonly IOpenAIService _openAIService;

        public WhisperSpeechRecognitionService(IOpenAIService openAIService)
        {
            _openAIService = openAIService ?? throw new ArgumentNullException(nameof(openAIService));
        }

        public async Task<string> RecognizeSpeechToTextAsync(
            byte[] voiceMessage, string fileName, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(voiceMessage);
            ArgumentNullException.ThrowIfNull(fileName);
            if (voiceMessage.Length <= 0)
            {
                throw new ArgumentException(nameof(voiceMessage));
            }

            var response = await _openAIService
                .Audio
                .CreateTranscription(
                    new AudioCreateTranscriptionRequest()
                    {
                        FileName = fileName,
                        File = voiceMessage,
                        Model = Models.WhisperV1,
                        ResponseFormat = StaticValues.AudioStatics.ResponseFormat.VerboseJson
                    },
                    cancellationToken);

            if (!response.Successful)
            {
                throw new InvalidSpeechRecognitionException("Can`t recognize speech to text!");
            }

            var textMessage = string.Join("\n", response.Text);
            return textMessage;
        }
    }
}
