using BuddyLanguage.Domain.Services;
using BuddyLanguage.TelegramBot.Commands;
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

    public TelegramBotUpdatesListener(
        ITelegramBotClient telegramBotClient,
        ILogger<TelegramBotUpdatesListener> logger,
        IServiceProvider serviceProvider)
    {
        _botClient = telegramBotClient ?? throw new ArgumentNullException(nameof(telegramBotClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
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

    private Task SendTypingActionAsync(ChatId chatId)
        => _botClient.SendChatActionAsync(chatId, ChatAction.Typing);

    private async Task UpdateHander(
        ITelegramBotClient telegramBotClient, Update update, CancellationToken cancellationToken)
    {
        await using var scope = _serviceProvider.CreateAsyncScope();
        var botCommandHandlers = scope.ServiceProvider
            .GetServices<IBotCommandHandler>().ToArray();

        //Chain of responsibility
        var commandHandler = botCommandHandlers.FirstOrDefault(handler => handler.CanHandleCommand(update));
        _logger.LogInformation("Received update of type {UpdateType}", update.Type);
        _logger.LogInformation("Command handler: {CommandHandler}", commandHandler?.GetType().Name);

        if (commandHandler != null && update.Message != null)
        {
            await SendTypingActionAsync(update.Message.Chat.Id); // Показываем, что робот печатает сообщение
            await _botClient.SendTextMessageAsync(
                update.Message.Chat.Id, "Дай подумать...", cancellationToken: cancellationToken);
            await commandHandler.HandleAsync(update, cancellationToken);
        }
        else
        {
            var unknownCommandHandler = botCommandHandlers.First(handler => handler is UnknownCommandHandler);
            await unknownCommandHandler.HandleAsync(update, cancellationToken);
        }
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

//public record User(string TelegramUserId);
