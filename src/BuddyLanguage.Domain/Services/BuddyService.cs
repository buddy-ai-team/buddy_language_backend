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
                string[]? Mistakes,
                string[]? Words)>
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
            var mistakesTask = GetGrammarMistakesAndLearningWords(
                userMessage, nativeLanguage, targetLanguage, cancellationToken);

            await Task.WhenAll(assistantTask, mistakesTask);

            var assistantAnswer = await assistantTask;
            var mistakes = await mistakesTask;

            _logger.LogDebug("Assistant answer: {AssistantAnswer}", assistantAnswer);

            if (mistakes!.WordsCount > 0)
            {
                await AddWordsToUser(mistakes.Words, user.Id, cancellationToken);
            }

            var botAnswerWavMessage = await _textToSpeechService.TextToWavByteArrayAsync(
                assistantAnswer, targetLanguage, voice, speed, cancellationToken);

            return (
                userMessage, assistantAnswer, botAnswerWavMessage, mistakes.GrammaMistakes, mistakes.Words);
        }

        public async Task<string> ContinueDialogAndGetAnswer(
            string textMessage, Guid userId, CancellationToken cancellationToken)
        {
            ArgumentException.ThrowIfNullOrEmpty(textMessage);
            return await _chatGPTService.GetAnswerOnTopic(
                textMessage, userId, cancellationToken);
        }

        public async Task<MistakesAnswer> GetGrammarMistakesAndLearningWords(
            string textMessage,
            Language nativeLanguage,
            Language learnedLanguage,
            CancellationToken cancellationToken)
        {
            ArgumentException.ThrowIfNullOrEmpty(textMessage);

            var prompt = $"Here's the text in {learnedLanguage}, it may contain {nativeLanguage} " +
                $"words. Imagine that you are my {learnedLanguage} teacher. " +
                $"After analyzing this text in {learnedLanguage}, highlight 2-3 gross grammatical " +
                $"errors yourself, if any, and write down the rules for these errors in the " +
                $"corresponding \"Mistakes\" field in {nativeLanguage}, as well as record the number " +
                $"of these errors in the \"MistakesCount\" field. " +
                $"Also, {nativeLanguage} words may also be present in this text, " +
                $"please do not count them for grammatical errors, count the number of " +
                $"{nativeLanguage} words and write them in the \"WordsCount\" field," +
                $"and the words themselves should be written in the \"Words\" field in {learnedLanguage}.";

            return await _chatGPTService.GetStructuredAnswer<MistakesAnswer>(
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
