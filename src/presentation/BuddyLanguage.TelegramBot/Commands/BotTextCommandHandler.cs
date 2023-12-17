using Telegram.Bot.Types;

namespace BuddyLanguage.TelegramBot.Commands;

public abstract class BotTextCommandHandler : IBotCommandHandler
{
    public abstract string Command { get; }

    public abstract int Order { get; }

    public virtual bool CanHandleCommand(Update update)
        => update.Message?.Text == Command;

    public abstract Task HandleAsync(Update update, CancellationToken cancellationToken);
}
