using Telegram.Bot;
using Telegram.Bot.Types;

namespace BuddyLanguage.TelegramBot.Commands;

public class SetBaseLanguageCommandHandler : BotCommandHandler
{
    public string Command => "/setBaseLanguage";

    private readonly ITelegramBotClient _botClient;

    public SetBaseLanguageCommandHandler(ITelegramBotClient botClient)
    {
        _botClient = botClient ?? throw new ArgumentNullException(nameof(botClient));
    }

    public Task HandleAsync(Update update, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(update);
        var message = update.Message;
        var telegramUserId = message.From!.Id;
        var messageText = message.Text;
        //....
        return Task.FromResult(true);
    }
}