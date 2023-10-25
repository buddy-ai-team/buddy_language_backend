using Telegram.Bot;
using Telegram.Bot.Types;
using File = Telegram.Bot.Types.File;

namespace BuddyLanguage.TelegramBot.Commands;

public class UserVoiceCommandHandler : BotCommandHandler
{
    public string? Command => null;

    private readonly ITelegramBotClient _botClient;
    private readonly ILogger<UserVoiceCommandHandler> _logger;
    private readonly UserService _userService;
    
    public UserVoiceCommandHandler(
        ITelegramBotClient botClient,
        ILogger<UserVoiceCommandHandler> logger,
        UserService userService)
    {
        _botClient = botClient ?? throw new ArgumentNullException(nameof(botClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _userService = userService ?? throw new ArgumentNullException(nameof(userService));
    }

    public async Task HandleAsync(Update update, CancellationToken cancellationToken)
    {
        var voice = update.Message?.Voice;
        var telegramUserId = update.Message.From!.Id;
        var user = _userService.GetUserByTelegramId(telegramUserId);
        //TODO здесь может быть проверка длительности voice.Duration - не более 5 минут
        File voiceFile = await _botClient.GetFileAsync(voice.FileId, cancellationToken);
        using var voiceStream = new MemoryStream(); //voiceFile.FileSize
        if (voiceFile.FilePath is {} filePath)
        {
            await _botClient.DownloadFileAsync(filePath, voiceStream, cancellationToken);
        }
        else
        {
            _logger.LogError("Can`t get file path for voice message! {@VoiceFile}", voiceFile);
        }
        var voiceMessage = voiceStream.ToArray();
        
        // TODO:
        // var (answerBytes, mistakes, words) = BuddyService.ProcessUserMessage(user, voiceMessage);
        // TODO: отправка ответа пользоваетелю
        using var memoryStream = new MemoryStream(answerBytes);
        await _botClient.SendVoiceAsync(
            chatId: update.Message.Chat.Id,
            voice: InputFile.FromStream(memoryStream, "answer.ogg"),
            caption: "Nice Voice Message",
            cancellationToken: cancellationToken);
    }
    
    public bool CanHandleCommand(Update update) 
        => update.Message?.Voice != null;
}