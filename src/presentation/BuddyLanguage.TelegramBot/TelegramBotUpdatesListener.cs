using BuddyLanguage.Domain.Services;
using BuddyLanguage.TelegramBot.Commands;
using BuddyLanguage.TelegramBot.Services;
using OpenAI.ChatGpt.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace BuddyLanguage.TelegramBot;

public class TelegramBotUpdatesListener : BackgroundService
{
    private readonly ITelegramBotClient _botClient;
    private readonly ILogger<TelegramBotUpdatesListener> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly TelegramUserRepository _telegramUserRepository;

    public TelegramBotUpdatesListener(
        ITelegramBotClient telegramBotClient,
        ILogger<TelegramBotUpdatesListener> logger,
        IServiceProvider serviceProvider,
        TelegramUserRepository telegramUserRepository)
    {
        _botClient = telegramBotClient ?? throw new ArgumentNullException(nameof(telegramBotClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        _telegramUserRepository = telegramUserRepository ?? throw new ArgumentNullException(nameof(telegramUserRepository));
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var options = new ReceiverOptions() { AllowedUpdates = Array.Empty<UpdateType>() };

        var me = await _botClient.GetMeAsync(stoppingToken);
        _logger.LogInformation("Start listening for @{BotName}", me.Username);

        // StartReceiving - Long Polling постоянно опрашивает сервера Telegram на предмет новых сообщений (на тредпуле)
        _botClient.StartReceiving(
            updateHandler: UpdateHander,
            pollingErrorHandler: ErrorHandler,
            options,
            cancellationToken: stoppingToken);
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

    private async Task UpdateHander(
        ITelegramBotClient telegramBotClient, Update update, CancellationToken cancellationToken)
    {
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

        await using var scope = _serviceProvider.CreateAsyncScope();
        var botCommandHandlers = scope.ServiceProvider
            .GetServices<IBotCommandHandler>().ToArray();

        //Chain of responsibility
        var commandHandler = botCommandHandlers.FirstOrDefault(handler => handler.CanHandleCommand(update));
        _logger.LogInformation("Received update of type {UpdateType}", update.Type);
        _logger.LogInformation("Command handler: {CommandHandler}", commandHandler?.GetType().Name);

        if (commandHandler != null && update.Message != null)
        {
            var ctsTyping = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            _ = SendDelayedTypingActionAsync(update.Message.Chat.Id, ctsTyping.Token); // Показываем, что робот печатает сообщение
            try
            {
                await commandHandler.HandleAsync(update, cancellationToken);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error in command handler {CommandHandler}", commandHandler.GetType().Name);
                await telegramBotClient.SendTextMessageAsync(
                    update.Message.Chat.Id,
                    $"Произошла ошибка:\n{e}",
                    cancellationToken: cancellationToken);
            }

            await ctsTyping.CancelAsync();
        }
        else
        {
            var unknownCommandHandler = botCommandHandlers.First(handler => handler is UnknownCommandHandler);
            await unknownCommandHandler.HandleAsync(update, cancellationToken);
        }
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

    private Task ErrorHandler(
        ITelegramBotClient telegramBotClient,
        Exception exception,
        CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "Error in TelegramBotUpdatesListener");
        return Task.CompletedTask;
    }
}
