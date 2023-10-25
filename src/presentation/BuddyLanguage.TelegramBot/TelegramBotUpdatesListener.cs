using BuddyLanguage.TelegramBot.Commands;
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
        // StartReceiving - Long Polling постоянно опрашивает сервера Telegram на предмет новых сообщений (на тредпуле)
        _botClient.StartReceiving(
            updateHandler: UpdateHander,
            pollingErrorHandler: ErrorHandler,
            options,
            cancellationToken: stoppingToken
        );
    }

    private async Task UpdateHander(ITelegramBotClient _, Update update, CancellationToken cancellationToken)
    {
        await using var scope = _serviceProvider.CreateAsyncScope();
        var botCommandHandlers = scope.ServiceProvider.GetServices<BotCommandHandler>();
        //Chain of responsibility
        var commandHandler = botCommandHandlers.FirstOrDefault(handler => handler.CanHandleCommand(update));
        if (commandHandler != null)
        {
            await commandHandler.HandleAsync(update, cancellationToken);
        }
        else
        {
            // TODO call UnknownCommandHandler:
        }
    }

    private Task ErrorHandler(ITelegramBotClient _, Exception exception, CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "Error in TelegramBotUpdatesListener");
        return Task.CompletedTask;
    }
}

public record User(string TelegramUserId); //TODO: заменить на реальную модель пользователя