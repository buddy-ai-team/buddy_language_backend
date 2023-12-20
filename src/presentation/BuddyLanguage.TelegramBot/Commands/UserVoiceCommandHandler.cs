using System.Diagnostics;
using System.Text;
using BuddyLanguage.Domain;
using BuddyLanguage.Domain.Enumerations;
using BuddyLanguage.Domain.Interfaces;
using BuddyLanguage.Domain.Services;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using File = Telegram.Bot.Types.File;

namespace BuddyLanguage.TelegramBot.Commands;

public class UserVoiceCommandHandler : IBotCommandHandler
{
    // Длительность больше минуты обрабатывается очень долго, выходит за таймаута WebHook'а телеграмма и бот не отвечает
    private static readonly TimeSpan _audioDurationLimit = TimeSpan.FromMinutes(1);

    private readonly ITelegramBotClient _botClient;
    private readonly ILogger<UserVoiceCommandHandler> _logger;
    private readonly UserService _userService;
    private readonly BuddyService _buddyService;
    private readonly IChatGPTService _chatGPTService;

    public UserVoiceCommandHandler(
        ITelegramBotClient botClient,
        ILogger<UserVoiceCommandHandler> logger,
        UserService userService,
        BuddyService buddyService,
        IChatGPTService chatGPTService)
    {
        _botClient = botClient ?? throw new ArgumentNullException(nameof(botClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        _buddyService = buddyService ?? throw new ArgumentNullException(nameof(buddyService));
        _chatGPTService = chatGPTService ?? throw new ArgumentNullException(nameof(chatGPTService));
    }

    public int Order => 0;

    public bool CanHandleCommand(Update update)
        => update.Message?.Voice != null;

    public async Task HandleAsync(Update update, CancellationToken cancellationToken)
    {
        var info = IBotCommandHandler.GetTelegramMessageBaseInfoOrThrow(update);

        var voice = update.Message!.Voice!;

        var user = await _userService.GetUserByTelegramId(info.UserId, cancellationToken);
        _logger.LogInformation($"Run {nameof(UserVoiceCommandHandler)}...");

        var nativeLanguage = user.UserPreferences.NativeLanguage;

        var thinkingMessage = await _botClient.SendTextMessageAsync(
            update.Message!.Chat.Id, "\u23f3 Analyzing...", cancellationToken: cancellationToken);

        var sw = Stopwatch.StartNew();
        try
        {
            var currentDuration = TimeSpan.FromSeconds(voice.Duration);
            if (currentDuration <= _audioDurationLimit)
            {
                File voiceFile = await _botClient.GetFileAsync(voice.FileId, cancellationToken);

                /* файл почти всегда будет весить больше 85 кб, поэтому в этом месте будет происходить дорогая аллокация в LOH.
                  Скорее всего, можно обойтись вообще без лишних аллокаций:
                    если пулить один раз буффер для MemoryStream, затем во все методы передавать этот MemoryStream
                  Или использовать это решение: Microsoft.IO.RecyclableMemoryStream
                */
                using var voiceStream = new MemoryStream((int)voiceFile.FileSize.GetValueOrDefault());

                if (voiceFile.FilePath is { } filePath)
                {
                    await _botClient.DownloadFileAsync(filePath, voiceStream, cancellationToken);
                }
                else
                {
                    _logger.LogError("Can`t get file path for voice message! {@VoiceFile}", voiceFile);
                    await _botClient.SendTextMessageAsync(
                        update.Message.Chat.Id,
                        "Can`t get file path for voice message!", // TODO LocalizationService
                        cancellationToken: cancellationToken);
                    return;
                }

                var userMessageResult =
                    await _buddyService.ProcessUserMessage(user, voiceStream.ToArray(), cancellationToken);

                await SendResultToUser(update, userMessageResult, cancellationToken);
            }
            else
            {
                await SendSpeechDurationExceededMessage(update, nativeLanguage, cancellationToken);
            }
        }
        finally
        {
            sw.Stop();
            await _botClient.EditMessageTextAsync(
                update.Message.Chat.Id,
                thinkingMessage.MessageId,
                "✅ Analyzed in " + sw.Elapsed.TotalSeconds.ToString("0.00") + "s",
                cancellationToken: cancellationToken);
        }
    }

    private async Task SendResultToUser(
        Update update,
        UserMessageProcessingResult userMessageResult,
        CancellationToken cancellationToken)
    {
        var answerText = userMessageResult.BotAnswerMessage;
        var answerBytes = userMessageResult.BotAnswerAudio;
        var pronunciationWordsBytes = userMessageResult.BadPronunciationAudio;
        var badPronunciationWords = userMessageResult.BadPronunciationWords;
        var mistakes = userMessageResult.GrammarMistakes;
        var mistakesBytes = userMessageResult.GrammarMistakesAudio;
        var words = userMessageResult.Words;

        await _botClient.SendTextMessageAsync(
            update.Message!.Chat.Id,
            $"\n```\n{userMessageResult.RecognizedUserMessage}\n```",
            parseMode: ParseMode.Markdown,
            cancellationToken: cancellationToken);

        await _botClient.SendTextMessageAsync(
            update.Message.Chat.Id,
            answerText,
            replyToMessageId: update.Message.MessageId,
            cancellationToken: cancellationToken);

        using var memoryStreamAnswer = new MemoryStream(answerBytes);
        await _botClient.SendVoiceAsync(
            chatId: update.Message.Chat.Id,
            voice: InputFile.FromStream(memoryStreamAnswer, "answer.ogg"),
            replyToMessageId: update.Message.MessageId,
            cancellationToken: cancellationToken);

        if (pronunciationWordsBytes != null)
        {
            using var memoryStreamPronunciation = new MemoryStream(pronunciationWordsBytes);
            await _botClient.SendVoiceAsync(
                chatId: update.Message.Chat.Id,
                voice: InputFile.FromStream(memoryStreamPronunciation, "answer.ogg"),
                cancellationToken: cancellationToken);
        }

        if (badPronunciationWords != null && badPronunciationWords.Count > 0)
        {
            var sb = new StringBuilder();
            foreach (var word in badPronunciationWords)
            {
                sb.Append(word.Key).Append(" - ").Append(word.Value).Append('\n');
            }

            string finalText = sb.ToString();
            await _botClient.SendTextMessageAsync(
                chatId: update.Message.Chat.Id,
                text: finalText,
                cancellationToken: cancellationToken);
        }

        if (mistakes.Length > 0 && words.Count > 0)
        {
            var sb = new StringBuilder();
            string grammaMistakes = string.Join(", ", mistakes);
            sb.Append(grammaMistakes);
            sb.Append('\n');
            foreach (var word in words)
            {
                sb.Append(word.Key).Append(" - ").Append(word.Value).Append('\n');
            }

            string finalText = sb.ToString();
            await _botClient.SendTextMessageAsync(
                chatId: update.Message.Chat.Id,
                text: finalText,
                cancellationToken: cancellationToken);
        }
        else if (mistakes.Length > 0 && words.Count == 0)
        {
            var grammarMistakes = string.Join(", ", mistakes);
            await _botClient.SendTextMessageAsync(
                chatId: update.Message.Chat.Id,
                text: $"{grammarMistakes}",
                cancellationToken: cancellationToken);
        }
        else if (mistakes.Length == 0 && words.Count > 0)
        {
            string studiedWords = string.Empty;
            foreach (var word in words)
            {
                studiedWords += $"{word.Key} - {word.Value}\n";
            }

            await _botClient.SendTextMessageAsync(
                chatId: update.Message.Chat.Id,
                text: $"{studiedWords}",
                cancellationToken: cancellationToken);
        }

        if (mistakesBytes != null)
        {
            using var memoryStreamMistakes = new MemoryStream(mistakesBytes);
            await _botClient.SendVoiceAsync(
                chatId: update.Message.Chat.Id,
                voice: InputFile.FromStream(memoryStreamMistakes, "mistakes.ogg"),
                cancellationToken: cancellationToken);
        }
    }

    private async Task SendSpeechDurationExceededMessage(
        Update update,
        Language nativeLanguage,
        CancellationToken cancellationToken)
    {
        string text = "Превышена допустимая длительность голосового сообщения. " +
                      "Максимальная длительность: 3 минуты.";

        var textInNativeLanguage = await _chatGPTService.GetTextTranslatedIntoNativeLanguage(
            text, Language.Russian, nativeLanguage, cancellationToken);
        await _botClient.SendTextMessageAsync(
            chatId: update.Message!.Chat.Id,
            text: textInNativeLanguage,
            cancellationToken: cancellationToken);
    }
}
