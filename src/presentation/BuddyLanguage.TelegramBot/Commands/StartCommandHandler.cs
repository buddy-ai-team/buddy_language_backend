using BuddyLanguage.Domain.Entities;
using BuddyLanguage.Domain.Services;
using BuddyLanguage.TextToSpeech;
using Microsoft.CognitiveServices.Speech.Transcription;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace BuddyLanguage.TelegramBot.Commands;

public class StartCommandHandler : IBotCommandHandler
{
    private readonly ITelegramBotClient _botClient;
    private readonly UserService _userService;

    public StartCommandHandler(ITelegramBotClient botClient, UserService userService)
    {
        _botClient = botClient ?? throw new ArgumentNullException(nameof(botClient));
        _userService = userService ?? throw new ArgumentNullException(nameof(userService));
    }

    public string Command => "/start";

    public async Task HandleAsync(Update update, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(update);
        var message = update.Message;
        if (message != null)
        {
            var telegramId = message.From!.Id.ToString();
            var firstName = message.From.FirstName;
            var lastName = message.From.LastName ?? string.Empty; // Присваиваем пустую строку, если lastName равно null
            var messageText = message.Text;
            bool isRegistered = false; // Переменная для проверки регистрации пользователя

            if (messageText == "/start")
            {
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
                    
                    using var memoryStream = new MemoryStream();
                    await _botClient.SendVoiceAsync(
                    chatId: telegramId,
                    voice: InputFile.FromStream(memoryStream, "answer.ogg"),
                    cancellationToken: cancellationToken);
                }
            }

            await _botClient.SendTextMessageAsync(message.Chat.Id, "Hello! I am Buddy! What are we going to talk about today?", cancellationToken: cancellationToken);
        }
    }
}
