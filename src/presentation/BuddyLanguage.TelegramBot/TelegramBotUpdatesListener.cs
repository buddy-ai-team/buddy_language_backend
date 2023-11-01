using BuddyLanguage.Domain.Services;
using BuddyLanguage.TelegramBot.Commands;
using BuddyLanguage.TelegramBot.Services;
using OpenAI.ChatGpt.Interfaces;
using Sentry;
using Serilog;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace BuddyLanguage.TelegramBot;

public class TelegramBotUpdatesListener : BackgroundService
{
    private readonly ITelegramBotClient _botClient;
    private readonly IServiceProvider _serviceProvider;
    private readonly BotUserStateService _userStateService;

    public TelegramBotUpdatesListener(
        ITelegramBotClient telegramBotClient,
        IServiceProvider serviceProvider,
        BotUserStateService userStateService)
    {
        _botClient = telegramBotClient ?? throw new ArgumentNullException(nameof(telegramBotClient));
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        _userStateService = userStateService ?? throw new ArgumentNullException(nameof(userStateService));
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var options = new ReceiverOptions() { AllowedUpdates = Array.Empty<UpdateType>() };

        var me = await _botClient.GetMeAsync(stoppingToken);
        Log.Logger.Information("Start listening for @{BotName}", me.Username);

        // StartReceiving - Long Polling постоянно опрашивает сервера Telegram на предмет новых сообщений (на тредпуле)
        _botClient.StartReceiving(
            updateHandler: UpdateHander,
            pollingErrorHandler: ErrorHandler,
            options,
            cancellationToken: stoppingToken);
    }

    private Task SendTypingActionAsync(ChatId chatId)
        => _botClient.SendChatActionAsync(chatId, ChatAction.Typing);

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
            Log.Logger.Information("Received update of unsupported type {UpdateType}", update.Type);
            return;
        }

        await using var scope = _serviceProvider.CreateAsyncScope();
        var botCommandHandlers = scope.ServiceProvider
            .GetServices<IBotCommandHandler>().ToArray();

        //Chain of responsibility
        var commandHandler = botCommandHandlers.FirstOrDefault(handler => handler.CanHandleCommand(update));
        Log.Logger.Information("Received update of type {UpdateType}", update.Type);
        Log.Logger.Information("Command handler: {CommandHandler}", commandHandler?.GetType().Name);

        if (commandHandler != null && update.Message != null)
        {
            await SendTypingActionAsync(update.Message.Chat.Id); // Показываем, что робот печатает сообщение
            await commandHandler.HandleAsync(update, cancellationToken);
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
        if (_userStateService.IsAuthenticated(telegramId))
        {
            return true;
        }

        if (_userStateService.GetUserState(telegramId) == UserBotStates.AccessPassword
            && message.Text is { } password)
        {
            if (!_userStateService.VerifyPasswordAndUpdateState(telegramId, password))
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
        _userStateService.SetUserState(message.From.Id, UserBotStates.AccessPassword);

        return false;
    }

    private Task ErrorHandler(
        ITelegramBotClient telegramBotClient,
        Exception exception,
        CancellationToken cancellationToken)
    {
        Log.Logger.Error(exception, "Error in TelegramBotUpdatesListener");
        SentrySdk.CaptureException(exception);
        return Task.CompletedTask;
    }
}
