using Telegram.Bot;
using Telegram.Bot.Types;

namespace BuddyLanguage.TelegramBot.Commands;

public class VersionCommandHandler : IBotCommandHandler
{
    private readonly ITelegramBotClient _botClient;

    public VersionCommandHandler(ITelegramBotClient botClient)
    {
        _botClient = botClient ?? throw new ArgumentNullException(nameof(botClient));
    }

    public string? Command => "/version";

    public async Task HandleAsync(Update update, CancellationToken cancellationToken)
    {
        var info = IBotCommandHandler.GetTelegramMessageBaseInfoOrThrow(update);
        var version = $"Ver: {ReflectionHelper.GetBuildDate():s}";
        await _botClient.SendTextMessageAsync(info.ChatId, version, cancellationToken: cancellationToken);
    }
}
