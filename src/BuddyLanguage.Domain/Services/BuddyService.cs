using BuddyLanguage.Domain.Entities;
using BuddyLanguage.Domain.Enumerations;
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

            var answerToQuestion = await Task.Run(() => GetAnswerToQuestion(textMessage, user.Id, cancellationToken));
            var mistakes = await Task.Run(() => GetGrammarMistakes(textMessage, cancellationToken));
            var learningWords = await Task.Run(() => GetLearningWords(textMessage, cancellationToken));

            var words = await ConvertStringToArray(learningWords, cancellationToken);
            await AddWordsToUser(words, user.Id, cancellationToken); 

            var voiceWavMessage = await _textToSpeechService.TextToWavByteArrayAsync(
                answerToQuestion, Language.English, Voice.Male, cancellationToken);

            return (voiceWavMessage, mistakes, learningWords);
        }

        private async Task AddWordsToUser(string[] words, Guid userId, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(words);
            if (words.Length <= 0)
            {
                throw new ArgumentException(nameof(words));
            }

            foreach (var word in words)
            {            
                await _wordService.AddWord(
                    userId, word, Language.English, WordEntityStatus.Learning, cancellationToken);
            }
        }

        private async Task<string> GetAnswerToQuestion(string textMessage, Guid userId, CancellationToken cancellationToken)
        {
            ArgumentException.ThrowIfNullOrEmpty(textMessage);
            var answerToQuestion = await _chatGPTService.GetAnswerOnTopic(textMessage, userId, cancellationToken);
            if (answerToQuestion is null)
            {
                throw new ArgumentNullException(nameof(answerToQuestion));
            }

            return answerToQuestion;
        }

        private async Task<string> GetGrammarMistakes(string textMessage, CancellationToken cancellationToken)
        {
            ArgumentException.ThrowIfNullOrEmpty(textMessage);
            var prompt =
                "Find grammatical errors in this text.Write the rules for these " +
                "grammatical errors.";
            var answerOfGramma = await _chatGPTService.GetAnswer(
                prompt, textMessage, cancellationToken);
            if (answerOfGramma is null)
            {
                throw new ArgumentNullException(nameof(answerOfGramma));
            }

            return answerOfGramma; 
        }

        private async Task<string> GetLearningWords(string textMessage, CancellationToken cancellationToken)
        {
            ArgumentException.ThrowIfNullOrEmpty(textMessage);
            var prompt =
                "Find Russian words in this text and write them in the following format: " +
                "russian word english translate separated by dash. If there are no Russian words," +
                "then write Good Job!";
            var learningWords = await _chatGPTService.GetAnswer(
                prompt, textMessage, cancellationToken);
            if (learningWords is null)
            {
                throw new ArgumentNullException(nameof(learningWords));
            }

            return learningWords;
        }

        private async Task<string[]> ConvertStringToArray(string textMessage, CancellationToken cancellationToken)
        {
            ArgumentException.ThrowIfNullOrEmpty(textMessage);
            var prompt =
                "Leave only English words from this text and separate them with a comma";
            var englishWords = await _chatGPTService.GetAnswer(
                prompt, textMessage, cancellationToken);
            if (englishWords is null)
            {
                throw new ArgumentNullException(nameof(englishWords));
            }

            var words = englishWords.Split(' ');
            return words;
        }
    }
}
