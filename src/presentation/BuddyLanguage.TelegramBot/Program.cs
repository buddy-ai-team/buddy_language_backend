using BuddyLanguage.Infrastructure;
using BuddyLanguage.TelegramBot;
using BuddyLanguage.TelegramBot.Commands;
using BuddyLanguage.TelegramBot.Services;
using OpenAI.ChatGpt;
using Polly;
using Sentry.AspNetCore;
using Serilog;
using Serilog.Events;
using Telegram.Bot;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplicationServices(builder.Configuration);

var token = builder.Configuration["BotConfiguration:Token"];
if (string.IsNullOrWhiteSpace(token))
{
    throw new InvalidOperationException("Telegram bot token is not set");
}

builder.Services.AddSingleton(new TelegramBotClientOptions(token));

// TODO Implement Polly after update to .NET 8: https://github.com/dotnet/docs/blob/main/docs/core/resilience/http-resilience.md
builder.Services.AddHttpClient("TelegramBotClient");
builder.Services.AddSingleton<ITelegramBotClient>(provider =>
{
    IHttpClientFactory factory = provider.GetRequiredService<IHttpClientFactory>();
    HttpClient client = factory.CreateClient("TelegramBotClient");
    return new TelegramBotClient(token, client);
});

builder.Services.AddSingleton<TelegramUserRepository>();

builder.Services.AddHostedService<TelegramBotUpdatesListener>();

builder.Services.AddScoped<IBotCommandHandler, StartCommandHandler>();
builder.Services.AddScoped<IBotCommandHandler, ResetTopicCommand>();
builder.Services.AddScoped<IBotCommandHandler, UnknownCommandHandler>();
builder.Services.AddScoped<IBotCommandHandler, UserVoiceCommandHandler>();
builder.Services.AddScoped<IBotCommandHandler, UserVoiceCommandHandler>();

// Настройка логирования с Serilog и Sentry
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console()
    .WriteTo.Sentry(o =>
    {
        o.Dsn = "https://70228ed8115b87d41a3cf0c17896d3bd@o4506146415837184.ingest.sentry.io/4506146476654592";
        o.MinimumBreadcrumbLevel = LogEventLevel.Debug;
        o.MinimumEventLevel = LogEventLevel.Error;
    })
    .CreateLogger();

builder.Host.UseSerilog((_, config) => config.ReadFrom.Configuration(builder.Configuration).WriteTo.Console());

builder.WebHost.UseSentry();

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();
