using Telegram.Bot;
using Telegram.Bot.Types;

namespace BuddyLanguage.TelegramBot.Commands;

public class StartCommandHandler : BotCommandHandler
{
    public string Command => "/start";
    
    private readonly ITelegramBotClient _botClient;

    public StartCommandHandler(ITelegramBotClient botClient)
    {
        _botClient = botClient ?? throw new ArgumentNullException(nameof(botClient));
    }

    public async Task HandleAsync(Update update, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(update);
        var message = update.Message;
        var telegramUserId = message.From!.Id;
        var messageText = message.Text;
        // TODO: await UserService.TryRegister(telegramUserId, message.From.FirstName, message.From.LastName);
        // TODO: сообщение о том, что пользователь зарегистрирован и краткое описание сервиса, а также в будущем здесь можно отправить первое аудиосообщение от бота
        await _botClient.SendTextMessageAsync(message.Chat.Id, "Hello! I am Buddy!", cancellationToken: cancellationToken);
    }
}