using System.Collections.Concurrent;
using BuddyLanguage.Domain.Entities;
using BuddyLanguage.Domain.Enumerations;
using BuddyLanguage.Domain.Exceptions;
using BuddyLanguage.Domain.Interfaces;

namespace BuddyLanguage.Domain.Services
{
    public class BuddyService
    {
        private readonly IChatGPTService _chatGPTService;
        private readonly ISpeechRecognitionService _speechRecognitionService;
        private readonly ITextToSpeech _textToSpeechService;
        private readonly WordEntityService _wordService;

        public BuddyService(
            IChatGPTService chatGPTService,
            ISpeechRecognitionService speechRecognitionService,
            ITextToSpeech textToSpeechService,
            WordEntityService wordService)
        {
            _chatGPTService = chatGPTService ?? throw new ArgumentNullException(nameof(chatGPTService));
            _speechRecognitionService = speechRecognitionService
                ?? throw new ArgumentNullException(nameof(speechRecognitionService));
            _textToSpeechService = textToSpeechService
                ?? throw new ArgumentNullException(nameof(textToSpeechService));
            _wordService = wordService
                ?? throw new ArgumentNullException(nameof(wordService));
        }

        public virtual async Task<(byte[] VoiceWavMessage, string Mistakes, string Words)>
            ProcessUserMessage(User user, byte[] voiceMessage, string fileName, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(user);
            ArgumentException.ThrowIfNullOrEmpty(fileName);
            if (voiceMessage.Length <= 0)
            {
                throw new ArgumentException(nameof(voiceMessage));
            }

            var textMessage = await _speechRecognitionService.RecognizeSpeechToTextAsync(
                voiceMessage, fileName, cancellationToken);
            if (textMessage is null)
            {
                throw new InvalidSpeechRecognitionException(nameof(voiceMessage));
            }

            var answerToQuestion = await Task.Run(() => GetAnswerToQuestion(textMessage, cancellationToken));
            var mistakes = await Task.Run(() => GetGrammaticalErrors(textMessage, cancellationToken));
            var learningWords = await Task.Run(() => GetLearningWords(textMessage, cancellationToken));

            //await _wordService.AddWord(
            //    user.Id, "слово", Language.English, WordEntityStatus.Learning, cancellationToken);
            var voiceWavMessage = await _textToSpeechService.TextToWavByteArrayAsync(
                answerToQuestion, Language.English, Voice.Male, cancellationToken);
            if (voiceMessage is null)
            {
                throw new InvalidTextToSpeechException(nameof(voiceMessage)); 
            }

            return (voiceWavMessage, mistakes, learningWords);
        }

        private async Task<string> GetAnswerToQuestion(string textMessage, CancellationToken cancellationToken)
        {
            ArgumentException.ThrowIfNullOrEmpty(textMessage);
            var answerPrompt = "Answer the question in English";
            var answerToQuestion = await _chatGPTService.GetAnswer(textMessage, answerPrompt, cancellationToken);
            if (answerToQuestion is null)
            {
                throw new ArgumentNullException(nameof(answerToQuestion));
            }

            return answerToQuestion;
        }

        private async Task<string> GetGrammaticalErrors(string textMessage, CancellationToken cancellationToken)
        {
            ArgumentException.ThrowIfNullOrEmpty(textMessage);
            var grammaPropmt =
                "Find grammatical errors in this text.Write the rules for these " +
                "grammatical errors.";
            var answerOfGramma = await _chatGPTService.GetAnswer(
                grammaPropmt, textMessage, cancellationToken);
            if (answerOfGramma is null)
            {
                throw new ArgumentNullException(nameof(answerOfGramma));
            }

            return answerOfGramma; 
        }

        private async Task<string> GetLearningWords(string textMessage, CancellationToken cancellationToken)
        {
            ArgumentException.ThrowIfNullOrEmpty(textMessage);
            var dictionary = new ConcurrentDictionary<string, string>();
            var russianWordsPropmt =
                "Find Russian words in this text and write them in the following format: " +
                "russian word - english translate";
            var learningWords = await _chatGPTService.GetAnswer(
                russianWordsPropmt, textMessage, cancellationToken);
            if (learningWords is null)
            {
                throw new ArgumentNullException(nameof(learningWords));
            }

            return learningWords;
        }
    }
}
