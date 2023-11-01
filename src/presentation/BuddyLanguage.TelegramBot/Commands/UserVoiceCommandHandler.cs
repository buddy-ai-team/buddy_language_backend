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

    public UserVoiceCommandHandler(
        ITelegramBotClient botClient,
        ILogger<UserVoiceCommandHandler> logger,
        UserService userService,
        BuddyService buddyService)
    {
        _botClient = botClient ?? throw new ArgumentNullException(nameof(botClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        _buddyService = buddyService ?? throw new ArgumentNullException(nameof(buddyService));
    }

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

        await _botClient.SendTextMessageAsync(
            update.Message!.Chat.Id, "Thinking...", cancellationToken: cancellationToken);

        var user = await _userService.GetUserByTelegramId(telegramUserId, cancellationToken);

        if (voice != null && update.Message != null)
        {
            var duration = TimeSpan.FromSeconds(voice.Duration);

            if (duration <= TimeSpan.FromMinutes(30))
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

                var (answerBytes, mistakes, words) =
                    await _buddyService.ProcessUserMessage(user, voiceMessage, cancellationToken);

                using var memoryStream = new MemoryStream(answerBytes); // доделать
                await _botClient.SendVoiceAsync(
                    chatId: update.Message.Chat.Id,
                    voice: InputFile.FromStream(memoryStream, "answer.ogg"),
                    cancellationToken: cancellationToken);
                await _botClient.SendTextMessageAsync(
                    chatId: update.Message.Chat.Id,
                    text: $"Ваши ошибки: {mistakes}",
                    cancellationToken: cancellationToken);
                await _botClient.SendTextMessageAsync(
                    chatId: update.Message.Chat.Id,
                    text: $"Слова на изучение: {words}",
                    cancellationToken: cancellationToken);
            }
            else
            {
                // Голосовое сообщение длится более 30 минут
                string text = "Превышена допустимая длительность голосового сообщения. " +
                                "Максимальная длительность: 30 минут.";
                await _botClient.SendTextMessageAsync(
                    chatId: update.Message.Chat.Id,
                    text: text,
                    cancellationToken: cancellationToken);
            }
        }
    }

    public bool CanHandleCommand(Update update)
        => update.Message?.Voice != null;
}
