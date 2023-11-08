using BuddyLanguage.Domain.Enumerations;
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

        /// <inheritdoc cref="ISpeechRecognitionService.RecognizeSpeechToTextAsync" />
        public async Task<string> RecognizeSpeechToTextAsync(
            byte[] voiceMessage,
            AudioFormat format,
            Language nativeLanguage,
            Language studiedLanguage,
            CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(voiceMessage);
            if (voiceMessage.Length == 0)
            {
                throw new ArgumentException("Voice message is empty", nameof(voiceMessage));
            }

            if (nativeLanguage == studiedLanguage)
            {
                throw new ArgumentException("Native language and studied language are the same");
            }

            var response = await _openAIService
                .Audio
                .CreateTranscription(
                    new AudioCreateTranscriptionRequest()
                {
                    FileName = $"voice.{format.ToString().ToLower()}",
                    File = voiceMessage,
                    Model = Models.WhisperV1,
                    ResponseFormat = StaticValues.AudioStatics.ResponseFormat.VerboseJson,
                    Prompt = $"In this audio, you will hear both {studiedLanguage} and {nativeLanguage} languages. Please transcribe exactly as spoken, including any filler words, without altering or omitting any part of the speech. It is crucial for the language learning application that each word, including those in Russian, is transcribed precisely as it is pronounced, maintaining all original features of the spoken content."
                },
                    cancellationToken);

            if (!response.Successful)
            {
                throw new InvalidSpeechRecognitionException(
                    "Error while recognizing speech: " + response.Error?.Message);
            }

            var textMessage = string.Join("\n", response.Text);
            return textMessage;
        }
    }
}
