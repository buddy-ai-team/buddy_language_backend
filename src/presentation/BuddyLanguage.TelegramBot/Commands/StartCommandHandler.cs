using BuddyLanguage.Domain.Services;
using Serilog;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace BuddyLanguage.TelegramBot.Commands;

public class StartCommandHandler : IBotCommandHandler
{
    private readonly ITelegramBotClient _botClient;
    private readonly UserService _userService;
    private readonly ILogger<StartCommandHandler> _logger;

    public StartCommandHandler(ITelegramBotClient botClient, UserService userService, ILogger<StartCommandHandler> logger)
    {
        _botClient = botClient ?? throw new ArgumentNullException(nameof(botClient));
        _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public string Command => "/start";

    public async Task HandleAsync(Update update, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(update);
        if (update.Message is { From: not null } message)
        {
            var telegramId = message.From.Id.ToString();
            var firstName = message.From.FirstName;
            var lastName = message.From.LastName ?? string.Empty; // Присваиваем пустую строку, если lastName равно null
            bool isRegistered = false; // Переменная для проверки регистрации пользователя

            _logger.LogInformation(
                "Run start command for {TelegramId} ({FirstName} {LastName})",
                telegramId,
                firstName,
                lastName);
            var user = await _userService.TryRegister(firstName, lastName, telegramId, cancellationToken);

            if (user != null)
            {
                isRegistered = true;
            }

            if (isRegistered)
            {
                const string welcomeMessage = "Привет! Поздравляю вас с регистрацией! Расскажу немного о себе, я ваш бот-собеседник. Вы можете отправлять голосовые сообщения на английском или русском языке не более 30 минут и я вам отвечу. Может поговорить на интересующие вас темы. Также я могу проводить грамматический анализ сообщений и исправлять ошибки.";
                await _botClient.SendTextMessageAsync(message.Chat.Id, welcomeMessage, cancellationToken: cancellationToken);

                // TODO: Отправка первого аудиосообщения от бота
                /*var welcomeBytes = await AzureTextToSpeech.TextToWavByteArrayAsync(welcomeMessage, Domain.Enumerations.Language.Russian, Domain.Enumerations.Voice.Female, cancellationToken: cancellationToken);*/

                // using var memoryStream = new MemoryStream();
                // await _botClient.SendVoiceAsync(
                // chatId: telegramId,
                // voice: InputFile.FromStream(memoryStream, "answer.ogg"),
                // cancellationToken: cancellationToken);
            }

            await _botClient.SendTextMessageAsync(message.Chat.Id, "Hello! I am Buddy! What are we going to talk about today?", cancellationToken: cancellationToken);
        }
    }
}
