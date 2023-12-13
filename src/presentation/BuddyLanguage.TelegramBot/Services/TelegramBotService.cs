using System.Text;
using BuddyLanguage.Domain.Exceptions.User;
using BuddyLanguage.Domain.Services;
using BuddyLanguage.TelegramBot.Commands;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace BuddyLanguage.TelegramBot.Services;

public class TelegramBotService
{
    private readonly ITelegramBotClient _botClient;
    private readonly UserService _userService;
    private readonly IEnumerable<IBotCommandHandler> _botCommandHandlers;
    private readonly ILogger<TelegramBotService> _logger;
    private readonly TelegramUserRepositoryInCache _telegramUserRepository;

    public TelegramBotService(
        ITelegramBotClient botClient,
        UserService userService,
        IEnumerable<IBotCommandHandler> botCommandHandlers,
        ILogger<TelegramBotService> logger,
        TelegramUserRepositoryInCache telegramUserRepository)
    {
        _botClient = botClient ?? throw new ArgumentNullException(nameof(botClient));
        _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        _botCommandHandlers = botCommandHandlers ?? throw new ArgumentNullException(nameof(botCommandHandlers));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _telegramUserRepository = telegramUserRepository ?? throw new ArgumentNullException(nameof(telegramUserRepository));
    }

    public Task HandleUpdate(Update update, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(update);
        return UpdateHandler(_botClient, update, cancellationToken);
    }

    public async Task UpdateHandler(
        ITelegramBotClient telegramBotClient, Update update, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(telegramBotClient);
        ArgumentNullException.ThrowIfNull(update);

        if (update.Message is { } message)
        {
            var authenticated = await EnsureAuthenticated(message, cancellationToken);
            if (!authenticated)
            {
                return;
            }
        }
        else if (update.Type != UpdateType.Message)
        {
            _logger.LogInformation("Received update of unsupported type {UpdateType}", update.Type);
            return;
        }

        //Chain of responsibility
        var commandHandler = _botCommandHandlers
            .OrderBy(it => it.Order)
            .First(handler => handler.CanHandleCommand(update));
        _logger.LogInformation(
            "Received update of type {UpdateType}, {MessageType}, {MessageText}, {From}, {CommandHandler}",
            update.Type,
            update.Message?.Type,
            update.Message?.Text,
            update.Message?.From?.Id,
            commandHandler.GetType().Name);

        var ctsTyping = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        _ = SendDelayedTypingActionAsync(update.Message!.Chat.Id, ctsTyping.Token); // Показываем, что робот печатает сообщение
        try
        {
            await commandHandler.HandleAsync(update, cancellationToken);
        }
        catch (UserNotFoundException) when (update.Message.From is not null)
        {
            _logger.LogInformation(
                "User {TelegramId} not found - registering",
                update.Message.From.Id);
            await RegisterUser(update, cancellationToken);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error in command handler {CommandHandler}", commandHandler.GetType().Name);
            await SendErrorToUser(telegramBotClient, update.Message.Chat.Id, e, cancellationToken);
        }

        await ctsTyping.CancelAsync();
    }

    private async Task SendDelayedTypingActionAsync(ChatId chatId, CancellationToken ctsTypingToken)
    {
        await Task.Delay(TimeSpan.FromMilliseconds(200), ctsTypingToken);
        while (!ctsTypingToken.IsCancellationRequested)
        {
            await _botClient.SendChatActionAsync(
                chatId, ChatAction.Typing, cancellationToken: ctsTypingToken);
            await Task.Delay(TimeSpan.FromSeconds(3), ctsTypingToken);
        }
    }

    private async Task SendErrorToUser(
        ITelegramBotClient telegramBotClient, long chatId, Exception e, CancellationToken cancellationToken)
    {
        // Convert the error message to bytes and write it to a MemoryStream
        byte[] errorMessageBytes = Encoding.UTF8.GetBytes($"Произошла ошибка:\n{e}");
        using var errorMessageStream = new MemoryStream(errorMessageBytes);

        // Send the error file
        await telegramBotClient.SendDocumentAsync(
            chatId: chatId,
            document: InputFile.FromStream(errorMessageStream, "error.txt"),
            caption: "Произошла ошибка",
            cancellationToken: cancellationToken);
    }

    private async Task RegisterUser(Update update, CancellationToken cancellationToken)
    {
        await _userService.TryRegister(
            update.Message!.From!.FirstName,
            update.Message.From.LastName,
            update.Message.From.Id.ToString(),
            cancellationToken);
    }

    private async Task<bool> EnsureAuthenticated(Message message, CancellationToken cancellationToken)
    {
        // TODO: to commands
        var telegramId = message.From!.Id;
        if (_telegramUserRepository.IsAuthenticated(telegramId))
        {
            return true;
        }

        if (_telegramUserRepository.GetUserState(telegramId) == UserBotStates.AccessPassword
            && message.Text is { } password)
        {
            if (!_telegramUserRepository.VerifyPasswordAndUpdateState(telegramId, password))
            {
                await _botClient.SendTextMessageAsync(
                    message.Chat.Id,
                    "Неверный пароль. Пожалуйста, повторите попытку.",
                    cancellationToken: cancellationToken);
                return false;
            }
            else
            {
                await _botClient.SendTextMessageAsync(
                    message.Chat.Id,
                    "Вы успешно авторизованы.",
                    cancellationToken: cancellationToken);
                message.Text = "/start"; //TODO: исправить костыль на выполнение команды StartCommandHandler
                return true;
            }
        }

        await _botClient.SendTextMessageAsync(
            message.Chat.Id,
            "Вы не авторизованы. Пожалуйста, авторизуйтесь.\nВведите пароль:",
            cancellationToken: cancellationToken);
        _telegramUserRepository.SetUserState(message.From.Id, UserBotStates.AccessPassword);

        return false;
    }
}
