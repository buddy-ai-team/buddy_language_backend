﻿using BuddyLanguage.Domain.Entities;
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
        private readonly WordService _wordService;
        private readonly ILogger<BuddyService> _logger;

        public BuddyService(
            IChatGPTService chatGPTService,
            ISpeechRecognitionService speechRecognitionService,
            ITextToSpeech textToSpeechService,
            IPronunciationAssessmentService pronunciationAssessmentService,
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
            _wordService = wordService
                ?? throw new ArgumentNullException(nameof(wordService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public virtual async Task<(
                string RecognizedMessage,
                string BotAnswerMessage,
                byte[] BotAnswerWavMessage,
                byte[] BotPronunciationWordsWavAnswer,
                string[] Mistakes,
                Dictionary<string, string> Words)>
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
            var pronunciationTask = _pronunciationAssessmentService.GetSpeechAssessmentAsync(
                oggVoiceMessage, targetLanguage.ToString(), cancellationToken);
            await Task.WhenAll(assistantTask, mistakesTask, wordsTask, pronunciationTask);

            var assistantAnswer = await assistantTask;
            var mistakes = await mistakesTask;
            var studiedWords = await wordsTask;
            var pronunciationWords = await pronunciationTask;

            var badPronouncedWords = GetWordsWithBadPronunciation(pronunciationWords);

            _logger.LogDebug("Assistant answer: {AssistantAnswer}", assistantAnswer);
            _logger.LogDebug("Assistant answer: {AssistantAnswer}", mistakes);

            if (studiedWords.WordsCount > 0)
            {
                await AddWordsToUser(studiedWords.StudiedWords, user.Id, cancellationToken);
            }

            var botPronunciationWordsWavAnswer = await GetPronunciationWordsWavMessage(
                targetLanguage, voice, speed, badPronouncedWords, cancellationToken);
            var botAnswerWavMessage = await _textToSpeechService.TextToWavByteArrayAsync(
                assistantAnswer, targetLanguage, voice, speed, cancellationToken);

            return (
                userMessage,
                assistantAnswer,
                botAnswerWavMessage,
                botPronunciationWordsWavAnswer,
                mistakes.GrammaMistakes,
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
                          $"на наличие {nativeLanguage} слов." +
                          $"Посчитай количество {nativeLanguage} слов, а также найди " +
                          $"абсолютно все {nativeLanguage} слова из текста, если они есть." +
                          $"Затем запиши эти слова, а также " +
                          $"перевод этих слов на {targetLanguage} в поле \"StudiedWords\"." +
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
            Dictionary<string, string> words,
            Guid userId,
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
                await _wordService.AddWord(
                    userId, word.Key, word.Value, Language.English, WordEntityStatus.Learning, cancellationToken);
            }
        }

        private IReadOnlyList<string> GetWordsWithBadPronunciation(
            IReadOnlyList<WordPronunciationAssessment> pronunciationWords)
        {
            ArgumentNullException.ThrowIfNull(pronunciationWords);
            double acceptableAccuracyScore = 75;
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

        private async Task<byte[]> GetPronunciationWordsWavMessage(
            Language targetLanguage,
            Voice voice,
            TtsSpeed speed,
            IReadOnlyList<string> badPronouncedWordsList,
            CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(badPronouncedWordsList);

            if (badPronouncedWordsList.Count != 0)
            {
                var badPronouncedWords = string.Join(",", badPronouncedWordsList);
                var textForBadPronunciation = "The pronunciation of the following words should be improved: ";
                return await _textToSpeechService.TextToWavByteArrayAsync(
                $"{textForBadPronunciation} {badPronouncedWords}", targetLanguage, voice, speed, cancellationToken);
            }

            var textForGoodPronunciation = "You have a good pronunciation!";
            return await _textToSpeechService.TextToWavByteArrayAsync(
            textForGoodPronunciation, targetLanguage, voice, speed, cancellationToken);
        }
    }
}
