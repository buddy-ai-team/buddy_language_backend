using BuddyLanguage.Domain.Entities;
using BuddyLanguage.Domain.Enumerations;
using BuddyLanguage.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace BuddyLanguage.Domain.Services
{
    public class BuddyService
    {
        private readonly IChatGPTService _chatGPTService;
        private readonly ISpeechRecognitionService _speechRecognitionService;
        private readonly ITextToSpeech _textToSpeechService;
        private readonly WordEntityService _wordService;
        private readonly ILogger<BuddyService> _logger;

        public BuddyService(
            IChatGPTService chatGPTService,
            ISpeechRecognitionService speechRecognitionService,
            ITextToSpeech textToSpeechService,
            WordEntityService wordService,
            ILogger<BuddyService> logger)
        {
            _chatGPTService = chatGPTService ?? throw new ArgumentNullException(nameof(chatGPTService));
            _speechRecognitionService = speechRecognitionService
                ?? throw new ArgumentNullException(nameof(speechRecognitionService));
            _textToSpeechService = textToSpeechService
                ?? throw new ArgumentNullException(nameof(textToSpeechService));
            _wordService = wordService
                ?? throw new ArgumentNullException(nameof(wordService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public virtual async Task<(byte[] VoiceWavMessage, string Mistakes, string Words)>
            ProcessUserMessage(User user, byte[] oggVoiceMessage, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(user);

            // TODO User.NativeLanguage
            var nativeLanguage = Language.Russian;
            var learnedLanguage = Language.English;

            if (oggVoiceMessage.Length == 0)
            {
                throw new ArgumentException(nameof(oggVoiceMessage));
            }

            var textMessage = await _speechRecognitionService.RecognizeSpeechToTextAsync(
                oggVoiceMessage, "voice.ogg", cancellationToken);
            _logger.LogDebug("Recognized text: {TextMessage}", textMessage);

            // TODO(Khristina): сделать выполнение параллельным
            var assistantAnswer = await GetAssistantAnswer(textMessage, user.Id, cancellationToken);
            var mistakes = await GetGrammarMistakes(textMessage, nativeLanguage,  learnedLanguage, cancellationToken);
            var learningWords = await GetLearningWords(textMessage, cancellationToken);

            _logger.LogDebug("Assistant answer: {AssistantAnswer}", assistantAnswer);
            _logger.LogDebug("Grammar mistakes: {Mistakes}", mistakes);
            _logger.LogDebug("Learning words: {LearningWords}", learningWords);

            var words = await ConvertStringToArray(learningWords, cancellationToken);
            await AddWordsToUser(words, user.Id, cancellationToken);

            var voiceWavMessage = await _textToSpeechService.TextToWavByteArrayAsync(
                assistantAnswer, learnedLanguage, Voice.Male, cancellationToken);

            return (voiceWavMessage, mistakes, learningWords);
        }

        public Task ResetTopic(User user, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(user);
            return _chatGPTService.ResetTopic(user.Id, cancellationToken);
        }

        private async Task AddWordsToUser(string[] words, Guid userId, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(words);
            if (words.Length == 0)
            {
                throw new ArgumentException(nameof(words));
            }

            foreach (var word in words)
            {
                //TODO: метод AddWords(words)
                await _wordService.AddWord(
                    userId, word, Language.English, WordEntityStatus.Learning, cancellationToken);
            }
        }

        private async Task<string> GetAssistantAnswer(
            string textMessage, Guid userId, CancellationToken cancellationToken)
        {
            ArgumentException.ThrowIfNullOrEmpty(textMessage);
            var answerToQuestion = await _chatGPTService.GetAnswerOnTopic(
                textMessage, userId, cancellationToken);

            return answerToQuestion;
        }

        private async Task<string> GetGrammarMistakes(
            string textMessage,
            Language nativeLanguage,
            Language learnedLanguage,
            CancellationToken cancellationToken)
        {
            ArgumentException.ThrowIfNullOrEmpty(textMessage);
            var prompt = $"Here is a text in {learnedLanguage} language." +
                         "Find grammar mistakes in this text. Write the rules for these " +
                         $"grammar mistakes. Answer in {nativeLanguage}.";
            var mistakes = await _chatGPTService.GetAnswer(
                prompt, textMessage, cancellationToken);

            return mistakes;
        }

        private async Task<string> GetLearningWords(string textMessage, CancellationToken cancellationToken)
        {
            ArgumentException.ThrowIfNullOrEmpty(textMessage);

            //TODO(Khristina): прокинуть языки в промпт
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

            //TODO(Khristina): прокинуть языки в промпт
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
