using BuddyLanguage.Domain.Interfaces;
using BuddyLanguage.Domain.Services;
using BuddyLanguage.TelegramBot.Models;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace BuddyLanguage.TelegramBot.Commands;

public class StartCommandHandler : IBotCommandHandler
{
    private readonly ITelegramBotClient _botClient;
    private readonly ITextToSpeech _textToSpeech;
    private readonly UserService _userService;
    private readonly ILogger<StartCommandHandler> _logger;

    public StartCommandHandler(
        ITelegramBotClient botClient,
        ITextToSpeech textToSpeech,
        UserService userService,
        ILogger<StartCommandHandler> logger)
    {
        _botClient = botClient ?? throw new ArgumentNullException(nameof(botClient));
        _textToSpeech = textToSpeech ?? throw new ArgumentNullException(nameof(textToSpeech));
        _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public string Command => "/start";

    public async Task HandleAsync(Update update, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(update);
        var info = IBotCommandHandler.GetTelegramMessageBaseInfoOrThrow(update);

        _logger.LogInformation(
            "Run start command for {TelegramId} ({FirstName} {LastName})",
            info.UserId,
            info.FirstName,
            info.LastName);

        var user = await _userService.TryRegister(
            info.FirstName, info.LastName, info.UserId, cancellationToken);
        var nativeLanguage = user.UserPreferences.NativeLanguage;
        var voice = user.UserPreferences.SelectedVoice;
        var speed = user.UserPreferences.SelectedSpeed;

        const string welcomeMessage =
            "Привет! Поздравляю вас с регистрацией! Расскажу немного о себе, я ваш бот-собеседник. Вы можете отправлять голосовые сообщения на английском или русском языке не более 3х минут и я вам отвечу. Может поговорить на интересующие вас темы. Также я могу проводить грамматический анализ сообщений и исправлять ошибки.";
        await _botClient.SendTextMessageAsync(
            info.ChatId, welcomeMessage, cancellationToken: cancellationToken);

        var welcomeMessageBytes = await _textToSpeech.TextToWavByteArrayAsync(
            welcomeMessage, nativeLanguage, voice, speed, cancellationToken);

        using var memoryStreamAnswer = new MemoryStream(welcomeMessageBytes);
        await _botClient.SendVoiceAsync(
            chatId: update.Message!.Chat.Id,
            voice: InputFile.FromStream(memoryStreamAnswer, "welcomMessage.ogg"),
            cancellationToken: cancellationToken);

        await _botClient.SendTextMessageAsync(
            info.ChatId,
            "Hello! I am Buddy! What are we going to talk about today?",
            cancellationToken: cancellationToken);
    }
}
