using BuddyLanguage.TelegramBot.Models;
using Telegram.Bot.Types;

namespace BuddyLanguage.TelegramBot.Commands;

public interface IBotCommandHandler
{
    int Order { get; }

    public bool CanHandleCommand(Update update);

    Task HandleAsync(Update update, CancellationToken cancellationToken);

    public static TelegramMessageBaseInfo GetTelegramMessageBaseInfoOrThrow(Update update)
    {
        ArgumentNullException.ThrowIfNull(update);
        ArgumentNullException.ThrowIfNull(update.Message);
        ArgumentNullException.ThrowIfNull(update.Message.From);
        ArgumentNullException.ThrowIfNull(update.Message.From.Id);
        ArgumentNullException.ThrowIfNull(update.Message.From.FirstName);

        return new TelegramMessageBaseInfo(
            update.Message.From.Id.ToString(),
            update.Message.Chat.Id,
            update.Message.From.FirstName,
            update.Message.From.LastName);
    }
}
