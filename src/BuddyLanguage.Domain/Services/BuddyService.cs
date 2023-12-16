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
        private readonly IPronunciationAssessmentService _pronunciationAssessmentService;
        private readonly IPromptService _promptService;
        private readonly WordService _wordService;
        private readonly ILogger<BuddyService> _logger;

        public BuddyService(
            IChatGPTService chatGPTService,
            ISpeechRecognitionService speechRecognitionService,
            ITextToSpeech textToSpeechService,
            IPronunciationAssessmentService pronunciationAssessmentService,
            IPromptService promptService,
            WordService wordService,
            ILogger<BuddyService> logger)
        {
            _chatGPTService = chatGPTService
                ?? throw new ArgumentNullException(nameof(chatGPTService));
            _speechRecognitionService = speechRecognitionService
                ?? throw new ArgumentNullException(nameof(speechRecognitionService));
            _textToSpeechService = textToSpeechService
                ?? throw new ArgumentNullException(nameof(textToSpeechService));
            _pronunciationAssessmentService = pronunciationAssessmentService
                ?? throw new ArgumentNullException(nameof(pronunciationAssessmentService));
            _promptService = promptService
                ?? throw new ArgumentNullException(nameof(promptService));
            _wordService = wordService
                ?? throw new ArgumentNullException(nameof(wordService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public virtual async Task<UserMessageProcessingResult>
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
            var role = user.UserPreferences.AssistantRole;
            var sourceLanguage = Language.Russian;

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
                userMessage, user.Id, role!, cancellationToken);
            var mistakesTask = GetGrammarMistakes(
                userMessage, nativeLanguage, cancellationToken);
            var wordsTask = GetLearningWords(
                userMessage, nativeLanguage, targetLanguage, cancellationToken);
            var botPronunciationTask = GetPronunciationWavMessageOrDefault(
                oggVoiceMessage,
                nativeLanguage,
                targetLanguage,
                voice,
                speed,
                sourceLanguage,
                cancellationToken);
            await Task.WhenAll(assistantTask, mistakesTask, wordsTask, botPronunciationTask);

            var assistantAnswer = await assistantTask;
            var mistakes = await mistakesTask;
            var studiedWords = await wordsTask;
            var botPronunciationWordsWavAnswer = await botPronunciationTask;

            _logger.LogDebug("Assistant answer: {AssistantAnswer}", assistantAnswer);
            _logger.LogDebug("Assistant answer: {AssistantAnswer}", mistakes);

            if (studiedWords.WordsCount > 0)
            {
                await AddWordsToUser(
                    studiedWords.StudiedWords, user.Id, targetLanguage, cancellationToken);
            }

            var botAnswerWavMessage = await _textToSpeechService.TextToByteArrayAsync(
                assistantAnswer, targetLanguage, voice, speed, cancellationToken);

            var grammarMistakes = string.Join(" ", mistakes.GrammarMistakes);
            var grammarMistakesAudio = mistakes.GrammaMistakesCount > 0 ? await _textToSpeechService.TextToByteArrayAsync(
                    grammarMistakes, targetLanguage, voice, speed, cancellationToken) : null;

            return new UserMessageProcessingResult(
                userMessage,
                assistantAnswer,
                botAnswerWavMessage,
                botPronunciationWordsWavAnswer,
                mistakes.GrammarMistakes,
                grammarMistakesAudio,
                studiedWords.StudiedWords);
        }

        public async Task<string> ContinueDialogAndGetAnswer(
            string textMessage, Guid userId, Role role, CancellationToken cancellationToken)
        {
            ArgumentException.ThrowIfNullOrEmpty(textMessage);
            ArgumentNullException.ThrowIfNull(role);
            return await _chatGPTService.GetAnswerOnTopic(
                textMessage, userId, role, cancellationToken);
        }

        public async Task<MistakesAnswer> GetGrammarMistakes(
            string textMessage,
            Language nativeLanguage,
            CancellationToken cancellationToken)
        {
            ArgumentException.ThrowIfNullOrEmpty(textMessage);
            var prompt = _promptService.GetPromptForGrammarMistakes(nativeLanguage);
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
            var prompt = _promptService.GetPromptForLearningWords(nativeLanguage, targetLanguage);
            return await _chatGPTService.GetStructuredAnswer<WordAnswer>(
                prompt, textMessage, cancellationToken);
        }

        public Task ResetTopic(User user, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(user);
            return _chatGPTService.ResetTopic(user.Id, cancellationToken);
        }

        private async Task<byte[]?> GetPronunciationWavMessageOrDefault(
            byte[] oggVoiceMessage, Language nativeLanguage, Language targetLanguage, Voice voice, TtsSpeed speed, Language sourceLanguage, CancellationToken cancellationToken)
        {
            if (oggVoiceMessage.Length == 0)
            {
                throw new ArgumentException(nameof(oggVoiceMessage));
            }

            if (_pronunciationAssessmentService.IsLanguageSupported(targetLanguage))
            {
                var pronunciationWords =
                    await _pronunciationAssessmentService.GetSpeechAssessmentFromOggOpus(
                        oggVoiceMessage, targetLanguage, cancellationToken);
                var badPronouncedWords = GetWordsWithBadPronunciation(pronunciationWords);
                return await GetPronunciationWordsWavMessageOrDefault(
                        sourceLanguage,
                        targetLanguage,
                        nativeLanguage,
                        voice,
                        speed,
                        badPronouncedWords,
                        cancellationToken);
            }

            return null;
        }

        private IReadOnlyList<string> GetWordsWithBadPronunciation(
            IReadOnlyList<WordPronunciationAssessment> pronunciationWords)
        {
            ArgumentNullException.ThrowIfNull(pronunciationWords);
            double acceptableAccuracyScore = 98;
            var badPronouncedWords = new List<string>();
            foreach (var word in pronunciationWords)
            {
                if (word.AccuracyScore <= acceptableAccuracyScore)
                {
                    badPronouncedWords.Add(word.Word);
                }
            }

            return badPronouncedWords;
        }

        private async Task<byte[]?> GetPronunciationWordsWavMessageOrDefault(
            Language sourceLanguage,
            Language targetLanguage,
            Language nativeLanguage,
            Voice voice,
            TtsSpeed speed,
            IReadOnlyList<string> badPronouncedWordsList,
            CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(badPronouncedWordsList);

            if (badPronouncedWordsList.Count == 0)
            {
                return null;
            }

            var badPronouncedWords = string.Join(",", badPronouncedWordsList);
            var textForBadPronunciation = "The pronunciation of the following words should be improved: ";
            var textForbadPronunciationInNativeLanguage = _chatGPTService.GetTextTranslatedIntoNativeLanguage(
                textForBadPronunciation, sourceLanguage, nativeLanguage, cancellationToken);
            return await _textToSpeechService.TextToByteArrayAsync(
            $"{textForbadPronunciationInNativeLanguage} {badPronouncedWords}", targetLanguage, voice, speed, cancellationToken);
        }

        private async Task AddWordsToUser(
            Dictionary<string, string> words,
            Guid userId,
            Language language,
            CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(words);
            if (words.Count == 0)
            {
                throw new ArgumentException(nameof(words));
            }

            foreach (var word in words)
            {
                //TODO: метод AddWords(words)
                var existedWord = await _wordService.FindWordByName(userId, word.Value, cancellationToken);
                if (existedWord == null)
                {
                    await _wordService.AddWord(
                        userId, word.Value, word.Key, language, WordEntityStatus.Learning, cancellationToken);
                }
            }
        }
    }
}
