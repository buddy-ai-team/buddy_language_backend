using BuddyLanguage.Domain.Entities;
using BuddyLanguage.Domain.Enumerations;
using BuddyLanguage.Domain.Exceptions;
using BuddyLanguage.Domain.GptDataModels.Answers;
using BuddyLanguage.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace BuddyLanguage.Domain.Services
{
    public class BuddyService
    {
        private readonly IChatGPTService _chatGPTService;
        private readonly ISpeechRecognitionService _speechRecognitionService;
        private readonly ITextToSpeech _textToSpeechService;
        private readonly IWordService _wordService;
        private readonly ILogger<BuddyService> _logger;

        public BuddyService(
            IChatGPTService chatGPTService,
            ISpeechRecognitionService speechRecognitionService,
            ITextToSpeech textToSpeechService,
            IWordService wordService,
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

        public virtual async Task<(
                string RecognizedMessage,
                string BotAnswerMessage,
                byte[] BotAnswerWavMessage,
                string[] Mistakes,
                string[] Words)>
            ProcessUserMessage(
                User user,
                byte[] oggVoiceMessage,
                CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(user);

            var nativeLanguage = user.UserPreferences.NativeLanguage;
            var targetLanguage = user.UserPreferences.TargetLanguage;
            var voice = user.UserPreferences.SelectedVoice;
            var speed = user.UserPreferences.SelectedSpeed;

            if (oggVoiceMessage.Length == 0)
            {
                throw new ArgumentException(nameof(oggVoiceMessage));
            }

            var userMessage = await _speechRecognitionService.RecognizeSpeechToTextAsync(
                oggVoiceMessage, AudioFormat.Ogg, nativeLanguage, targetLanguage, cancellationToken);
            _logger.LogWarning("Recognized text: {TextMessage}", userMessage);

            if (string.IsNullOrWhiteSpace(userMessage))
            {
                throw new RecognizedTextIsEmptyException("Can`t recognize user message");
            }

            var assistantTask = ContinueDialogAndGetAnswer(
                userMessage, user.Id, cancellationToken);
            var mistakesTask = GetGrammarMistakes(
                userMessage, nativeLanguage, cancellationToken);
            var wordsTask = GetLearningWords(
                userMessage, nativeLanguage, targetLanguage, cancellationToken);

            await Task.WhenAll(assistantTask, mistakesTask, wordsTask);

            var assistantAnswer = await assistantTask;
            var mistakes = await mistakesTask;
            var studiedWords = await wordsTask;

            _logger.LogDebug("Assistant answer: {AssistantAnswer}", assistantAnswer);
            _logger.LogDebug("Assistant answer: {AssistantAnswer}", mistakes);
            _logger.LogDebug("Assistant answer: {AssistantAnswer}", studiedWords);

            if (studiedWords.WordsCount > 0)
            {
                await AddWordsToUser(studiedWords.Words, user.Id, cancellationToken);
            }

            var botAnswerWavMessage = await _textToSpeechService.TextToWavByteArrayAsync(
                assistantAnswer, targetLanguage, voice, speed, cancellationToken);

            return (
                userMessage, assistantAnswer, botAnswerWavMessage, mistakes.GrammaMistakes, studiedWords.Words);
        }

        public async Task<string> ContinueDialogAndGetAnswer(
            string textMessage, Guid userId, CancellationToken cancellationToken)
        {
            ArgumentException.ThrowIfNullOrEmpty(textMessage);
            return await _chatGPTService.GetAnswerOnTopic(
                textMessage, userId, cancellationToken);
        }

        public async Task<MistakesAnswer> GetGrammarMistakes(
            string textMessage,
            Language nativeLanguage,
            CancellationToken cancellationToken)
        {
            ArgumentException.ThrowIfNullOrEmpty(textMessage);
            var prompt = $"Я хочу чтобы ты выступил в роли корректора. " +
                    $"Найди очень точно и верно грамматические ошибки (максимум 3) в этом тексте " +
                    $"и сформулируй их подбробно на {nativeLanguage} языке, " +
                    $"а также улучшения и исправления текста." +
                    $"Не нужно воспринимать за грамматические ошибки слова, " +
                    $"написанные на {nativeLanguage} языке." +
                    $"Запиши их в поле \"GrammaMistakes\".";
            return await _chatGPTService.GetStructuredAnswer<MistakesAnswer>(
                prompt, textMessage, cancellationToken);
        }

        public async Task<WordAnswer> GetLearningWords(
            string textMessage,
            Language nativeLanguage,
            Language targetLanguage, 
            CancellationToken cancellationToken)
        {
            ArgumentException.ThrowIfNullOrEmpty(textMessage);
            var prompt = $"Я предоставлю тебе тексты, которые ты должен будешь проверить дважды " +
            $"на наличие {nativeLanguage} слов. По завершении проверки текста, посчитай количество" +
            $" {nativeLanguage} слов, а также предоставь мне абсолютно все {nativeLanguage} слова" +
            $" из текста, если они есть. Затем переведи эти слова на {targetLanguage}язык и " +
            $"запиши переведенные слова в поле \"Words\". " +
            $"Пожалуйста, убедись, что ты нашел абсолютно все слова.";
            return await _chatGPTService.GetStructuredAnswer<WordAnswer>(
                prompt, textMessage, cancellationToken);
        }

        public Task ResetTopic(User user, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(user);
            return _chatGPTService.ResetTopic(user.Id, cancellationToken);
        }

        private async Task AddWordsToUser(
            string[] words,
            Guid userId,
            CancellationToken cancellationToken)
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
    }
}
