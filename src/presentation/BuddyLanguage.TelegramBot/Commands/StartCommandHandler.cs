using BuddyLanguage.Domain.Interfaces;
using BuddyLanguage.Domain.Services;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace BuddyLanguage.TelegramBot.Commands;

public class StartCommandHandler : IBotCommandHandler
{
    private readonly ITelegramBotClient _botClient;
    private readonly UserService _userService;
    private readonly ITextToSpeech _textToSpeech;
    private readonly IChatGPTService _chatGPTService;
    private readonly ILogger<StartCommandHandler> _logger;

    public StartCommandHandler(
        ITelegramBotClient botClient,
        UserService userService,
        ITextToSpeech textToSpeech,
        IChatGPTService chatGPTService,
        ILogger<StartCommandHandler> logger)
    {
        _botClient = botClient ?? throw new ArgumentNullException(nameof(botClient));
        _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        _textToSpeech = textToSpeech ?? throw new ArgumentNullException(nameof(textToSpeech));
        _chatGPTService = chatGPTService ?? throw new ArgumentNullException(nameof(chatGPTService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public int Order => 0;

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
        var targetLanguage = user.UserPreferences.TargetLanguage;
        var voice = user.UserPreferences.SelectedVoice;
        var speed = user.UserPreferences.SelectedSpeed;

        const string welcomeMessage =
             "Привет! Поздравляю вас с регистрацией! Расскажу немного о себе, " +
             "я ваш виртуальный собеседник. Со мной вы можете изучать язык методом разговорной практики. " +
             "Отправляйте мне голосовое сообщение на любом из поддерживаемых языков, " +
             "список которых вы можете посмотреть в настройках, и я вам отвечу. " +
             "Голосовые сообщения могут быть продолжительностью не более трёх минут. " +
             "Я могу поговорить на интересующие вас темы, а также я умею проводить " +
             "грамматический анализ сообщений, анализ произношения на иностранном языке" +
             " и исправлять найденные ошибки.";

        var welcomeMessageInNativeLanguage = await _chatGPTService.GetTextTranslatedIntoNativeLanguage(
            welcomeMessage, nativeLanguage, targetLanguage, cancellationToken);

        await _botClient.SendTextMessageAsync(
            info.ChatId, welcomeMessageInNativeLanguage, cancellationToken: cancellationToken);

        var welcomeMessageBytes = await _textToSpeech.TextToByteArrayAsync(
            welcomeMessageInNativeLanguage, nativeLanguage, voice, speed, cancellationToken);

        using var memoryStreamAnswer = new MemoryStream(welcomeMessageBytes);
        await _botClient.SendVoiceAsync(
            chatId: update.Message!.Chat.Id,
            voice: InputFile.FromStream(memoryStreamAnswer, "welcomeMessage.ogg"),
            cancellationToken: cancellationToken);
    }
}
