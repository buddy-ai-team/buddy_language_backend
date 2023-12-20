using System.Runtime;
using BuddyLanguage.TelegramBot.Services;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace BuddyLanguage.TelegramBot;

public class TelegramBotUpdatesListener : BackgroundService
{
    private readonly ITelegramBotClient _botClient;
    private readonly ILogger<TelegramBotUpdatesListener> _logger;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public TelegramBotUpdatesListener(
        ITelegramBotClient telegramBotClient,
        ILogger<TelegramBotUpdatesListener> logger,
        IServiceScopeFactory serviceScopeFactory)
    {
        _botClient = telegramBotClient ?? throw new ArgumentNullException(nameof(telegramBotClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _serviceScopeFactory = serviceScopeFactory ?? throw new ArgumentNullException(nameof(serviceScopeFactory));
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

    private async Task UpdateHander(
        ITelegramBotClient telegramBotClient, Update update, CancellationToken cancellationToken)
    {
        await using var scope = _serviceScopeFactory.CreateAsyncScope();
        var telegramBotService = scope.ServiceProvider.GetRequiredService<TelegramBotService>();
        await telegramBotService.UpdateHandler(telegramBotClient, update, cancellationToken);
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
