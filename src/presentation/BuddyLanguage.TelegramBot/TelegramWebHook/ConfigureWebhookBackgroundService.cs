using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace BuddyLanguage.TelegramBot.TelegramWebHook;

public class ConfigureWebhookBackgroundService : IHostedService
{
    private readonly ILogger<ConfigureWebhookBackgroundService> _logger;
    private readonly ITelegramBotClient _botClient;
    private readonly BotConfiguration _botConfig;

    public ConfigureWebhookBackgroundService(
        ILogger<ConfigureWebhookBackgroundService> logger,
        ITelegramBotClient botClient,
        IOptions<BotConfiguration> botOptions)
    {
        ArgumentNullException.ThrowIfNull(botOptions);
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _botClient = botClient ?? throw new ArgumentNullException(nameof(botClient));
        _botConfig = botOptions.Value;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        var webhookAddress = GetWebhookAddress();
        _logger.LogInformation("Setting webhook: {WebhookAddress}", webhookAddress);
        return _botClient.SetWebhookAsync(
            url: webhookAddress,
            allowedUpdates: Array.Empty<UpdateType>(),
            secretToken: _botConfig.Token,
            cancellationToken: cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Removing webhook");
        return _botClient.DeleteWebhookAsync(cancellationToken: cancellationToken);
    }

    private string GetWebhookAddress()
    {
        var host = _botConfig.WebHookHost;
        var route = GetControllerRoute(typeof(TelegramBotController));
        return host + route;
    }

    private string GetControllerRoute(Type controllerType)
    {
        var routeAttribute = controllerType.GetCustomAttribute<RouteAttribute>();
        return routeAttribute?.Template ?? throw new InvalidOperationException(
            $"Unable to find route for controller {controllerType.Name}");
    }
}
