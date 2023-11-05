using Telegram.Bot.Types;

namespace BuddyLanguage.TelegramBot.Commands;

public interface IBotCommandHandler
{
    string? Command { get; }

    Task HandleAsync(Update update, CancellationToken cancellationToken);

    public bool CanHandleCommand(Update update) => update.Message?.Text == Command;
}
