using Telegram.Bot;
using Telegram.Bot.Types;

namespace BuddyLanguage.TelegramBot.Commands;

public class StartCommandHandler : BotCommandHandler
{
    public string Command => "/start";
    
    private readonly ITelegramBotClient _botClient;
	private readonly UserService _userService; // ждём реализацию

	public StartCommandHandler(ITelegramBotClient botClient)
    {
        _botClient = botClient ?? throw new ArgumentNullException(nameof(botClient));
		_userService = userService ?? throw new ArgumentNullException(nameof(userService));
	}

    public async Task HandleAsync(Update update, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(update);
        var message = update.Message;
        var telegramUserId = message.From!.Id;
        var messageText = message.Text;

		var isFirstTimeRegistration = _userService.CheckIfFirstTimeRegistration(telegramUserId);
		if (isFirstTimeRegistration)
		{
			// TODO: await _userService.TryRegister(telegramUserId, message.From.FirstName, message.From.LastName);

			const string welcomeMessage = "Привет! Поздравляю вас с регистрацией! Расскажу немного о себе, я ваш бот-собеседник. Вы можете отправлять голосовые сообщения на английском или русском языке не более 30 минут и я вам отвечу. Может поговорить на интересующие вас темы. Также я могу проводить грамматический анализ сообщений и исправлять ошибки.";
			// TODO: Отправка сообщения о том, что пользователь зарегистрирован и краткое описание сервиса

			// TODO: Отправка первого аудиосообщения от бота
		}

		await _botClient.SendTextMessageAsync(message.Chat.Id, "Hello! I am Buddy! What are we going to talk about today?", cancellationToken: cancellationToken);
    }
}