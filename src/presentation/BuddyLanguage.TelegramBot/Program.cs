using BuddyLanguage.Infrastructure;
using BuddyLanguage.TelegramBot;
using BuddyLanguage.TelegramBot.Commands;
using BuddyLanguage.TelegramBot.Services;
using OpenAI.ChatGpt;
using Polly;
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
builder.Services.AddSingleton<ITelegramBotClient>(new TelegramBotClient(token));

builder.Services.AddSingleton<BotUserStateService>();

builder.Services.AddHostedService<TelegramBotUpdatesListener>();

builder.Services.AddScoped<IBotCommandHandler, StartCommandHandler>();
builder.Services.AddScoped<IBotCommandHandler, ResetTopicCommand>();
builder.Services.AddScoped<IBotCommandHandler, UnknownCommandHandler>();
builder.Services.AddScoped<IBotCommandHandler, UserVoiceCommandHandler>();
builder.Services.AddScoped<IBotCommandHandler, UserVoiceCommandHandler>();

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();

/// <summary>
/// Dummy class to make WebApplication.CreateBuilder() work
/// </summary>
// ReSharper disable once ClassNeverInstantiated.Global
public partial class Program
{
}
