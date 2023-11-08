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
                string? Mistakes,
                string? Words)>
            ProcessUserMessage(
                User user,
                byte[] oggVoiceMessage,
                CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(user);

            // TODO User.NativeLanguage
            var nativeLanguage = Language.Russian;
            var learnedLanguage = Language.English;

            if (oggVoiceMessage.Length == 0)
            {
                throw new ArgumentException(nameof(oggVoiceMessage));
            }

            var userMessage = await _speechRecognitionService.RecognizeSpeechToTextAsync(
                oggVoiceMessage, AudioFormat.Ogg, nativeLanguage, learnedLanguage, cancellationToken);
            _logger.LogWarning("Recognized text: {TextMessage}", userMessage);

            if (string.IsNullOrWhiteSpace(userMessage))
            {
                throw new RecognizedTextIsEmptyException("Can`t recognize user message");
            }

            var assistantAnswerTask = ContinueDialogAndGetAnswer(
                userMessage, user.Id, cancellationToken);
            var mistakesTask = FindGrammarMistakes(
                userMessage, nativeLanguage, learnedLanguage, cancellationToken);
            var studiedWordsTask = FindLearningWords(
                userMessage, nativeLanguage, cancellationToken);
            await Task.WhenAll(assistantAnswerTask, mistakesTask, studiedWordsTask);

            string assistantAnswer = await assistantAnswerTask;
            MistakesAnswer? mistakesAnswer = await mistakesTask;
            StudiedWordsAnswer? studiedWordsAnswer = await studiedWordsTask;

            var studiedWords = studiedWordsAnswer!.WordsCount > 0
               ? studiedWordsAnswer.ToString() : null;
            var mistakes = mistakesAnswer!.MistakesCount > 0
                ? mistakesTask.ToString() : null;

            _logger.LogDebug("Assistant answer: {AssistantAnswer}", assistantAnswer);
            _logger.LogDebug("Grammar mistakes: {@Mistakes}", mistakes);
            _logger.LogDebug("Studied words: {LearningWords}", studiedWords);

            if (studiedWordsAnswer is not null)
            {
                await AddWordsToUser(studiedWordsAnswer.Words, user.Id, cancellationToken);
            }

            var botAnswerWavMessage = await _textToSpeechService.TextToWavByteArrayAsync(
                assistantAnswer, learnedLanguage, Voice.Male, cancellationToken);

            return (userMessage, assistantAnswer, botAnswerWavMessage, mistakes, studiedWords);
        }

        public async Task<string> ContinueDialogAndGetAnswer(
            string textMessage, Guid userId, CancellationToken cancellationToken)
        {
            ArgumentException.ThrowIfNullOrEmpty(textMessage);
            return await _chatGPTService.GetAnswerOnTopic(
                textMessage, userId, cancellationToken);
        }

        public async Task<MistakesAnswer?> FindGrammarMistakes(
            string textMessage,
            Language nativeLanguage,
            Language learnedLanguage,
            CancellationToken cancellationToken)
        {
            ArgumentException.ThrowIfNullOrEmpty(textMessage);
            var prompt = $"Here is a text in {learnedLanguage} language." +
                         $"Find grammatical errors in this text, not spelling." +
                         $"Also, this text may include {nativeLanguage} words, " +
                         $"then you do not need to consider them as grammatical errors. " +
                         $"Write the rules for these " +
                         $"grammar mistakes. Answer in {nativeLanguage}.";
            var mistakes = await _chatGPTService.GetStructuredAnswer<MistakesAnswer>(
                prompt, textMessage, cancellationToken);

            return mistakes.MistakesCount > 0 ? mistakes : null;
        }

        public async Task<StudiedWordsAnswer?> FindLearningWords(
            string textMessage,
            Language nativeLanguage,
            CancellationToken cancellationToken)
        {
            ArgumentException.ThrowIfNullOrEmpty(textMessage);
            var prompt = $"This text may contain {nativeLanguage} words. " +
                $"If it is: how many {nativeLanguage} words does this text contain " +
                $"and what are them?";
            var answer = await _chatGPTService.GetStructuredAnswer<StudiedWordsAnswer>(
                prompt, textMessage, cancellationToken);

            return answer.WordsCount > 0 ? answer : null;
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
