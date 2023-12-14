using BuddyLanguage.Domain.Interfaces;
using BuddyLanguage.Domain.Services;
using Telegram.Bot;
using Telegram.Bot.Types;
using File = Telegram.Bot.Types.File;

namespace BuddyLanguage.TelegramBot.Commands;

public class UserVoiceCommandHandler : IBotCommandHandler
{
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

    public string? Command => null;

    public async Task HandleAsync(Update update, CancellationToken cancellationToken)
    {
        var voice = update.Message?.Voice;
        var telegramUserId = update.Message?.From?.Id.ToString();
        if (telegramUserId is null)
        {
            _logger.LogError("Can`t get telegram user id from update message! {@Update}", update);
            return;
        }

        var user = await _userService.GetUserByTelegramId(telegramUserId, cancellationToken);
        _logger.LogInformation("Processing UserVoiceCommand...");

        var nativeLanguage = user.UserPreferences.NativeLanguage;
        var targetLanguage = user.UserPreferences.TargetLanguage;

        var treatment = await _chatGPTService.GetTextTranslatedIntoNativeLanguage(
            "Обработка...", nativeLanguage, targetLanguage, cancellationToken);

        await _botClient.SendTextMessageAsync(
            update.Message!.Chat.Id, treatment, cancellationToken: cancellationToken);

        if (voice != null && update.Message != null)
        {
            var duration = TimeSpan.FromSeconds(voice.Duration);

            if (duration <= TimeSpan.FromMinutes(3))
            {
                File voiceFile = await _botClient.GetFileAsync(voice.FileId, cancellationToken);
                using var voiceStream = new MemoryStream(); //voiceFile.FileSize

                if (voiceFile.FilePath is { } filePath)
                {
                    await _botClient.DownloadFileAsync(filePath, voiceStream, cancellationToken);
                }
                else
                {
                    _logger.LogError("Can`t get file path for voice message! {@VoiceFile}", voiceFile);
                }

                var voiceMessage = voiceStream.ToArray();

                var userMessageResult =
                        await _buddyService.ProcessUserMessage(user, voiceMessage, cancellationToken);
                var recognizedMessage = userMessageResult.RecognizedMessage;
                var answerText = userMessageResult.BotAnswerMessage;
                var answerBytes = userMessageResult.BotAnswerWavMessage;
                var pronunciationWordsBytes = userMessageResult.BotPronunciationWordsWavAnswer;
                var mistakes = userMessageResult.Mistakes;
                var mistakesBytes = userMessageResult.MistakesWavAnswer;
                var words = userMessageResult.Words;

                await _botClient.SendTextMessageAsync(
                    update.Message.Chat.Id,
                    $"\n```\n{recognizedMessage}\n```",
                    cancellationToken: cancellationToken);
                await _botClient.SendTextMessageAsync(
                    update.Message.Chat.Id, answerText, cancellationToken: cancellationToken);

                using var memoryStreamAnswer = new MemoryStream(answerBytes);
                await _botClient.SendVoiceAsync(
                    chatId: update.Message.Chat.Id,
                    voice: InputFile.FromStream(memoryStreamAnswer, "answer.ogg"),
                    cancellationToken: cancellationToken);

                if (pronunciationWordsBytes != null)
                {
                    using var memoryStreamPronunciation = new MemoryStream(pronunciationWordsBytes);
                    await _botClient.SendVoiceAsync(
                        chatId: update.Message.Chat.Id,
                        voice: InputFile.FromStream(memoryStreamPronunciation, "answer.ogg"),
                        cancellationToken: cancellationToken);
                }

                if (mistakes.Length > 0 && words.Count > 0)
                {
                    var grammaMistakes = string.Join(", ", mistakes);
                    string studiedWords = string.Empty;
                    foreach (var word in words)
                    {
                        studiedWords += $"{word.Key} - {word.Value}\n";
                    }

                    await _botClient.SendTextMessageAsync(
                        chatId: update.Message.Chat.Id,
                        text: $"{grammaMistakes}\n{studiedWords}",
                        cancellationToken: cancellationToken);
                }
                else if (mistakes.Length > 0 && words.Count == 0)
                {
                    var grammaMistakes = string.Join(", ", mistakes);
                    await _botClient.SendTextMessageAsync(
                        chatId: update.Message.Chat.Id,
                        text: $"{grammaMistakes}",
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
            else
            {
                string text = "Превышена допустимая длительность голосового сообщения. " +
                                    "Максимальная длительность: 3 минуты.";

                var textInNativeLanguage = await _chatGPTService.GetTextTranslatedIntoNativeLanguage(
                    text, nativeLanguage, targetLanguage, cancellationToken);
                await _botClient.SendTextMessageAsync(
                    chatId: update.Message.Chat.Id,
                    text: textInNativeLanguage,
                    cancellationToken: cancellationToken);
            }
        }
    }

    public bool CanHandleCommand(Update update)
        => update.Message?.Voice != null;
}
