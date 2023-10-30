using BuddyLanguage.TelegramBot;
using BuddyLanguage.TelegramBot.Commands;
using Telegram.Bot;

var builder = WebApplication.CreateBuilder(args);

var token = builder.Configuration["BotConfiguration:BotToken"];
if (string.IsNullOrEmpty(token))
{
    throw new InvalidOperationException("Telegram bot token is not set");
}

builder.Services.AddSingleton<ITelegramBotClient>(new TelegramBotClient(token));
builder.Services.AddHostedService<TelegramBotUpdatesListener>();

builder.Services.AddSingleton<IBotCommandHandler, StartCommandHandler>();
builder.Services.AddSingleton<IBotCommandHandler, UnknownCommandHandler>();
builder.Services.AddSingleton<IBotCommandHandler, UserVoiceCommandHandler>();
builder.Services.AddSingleton<IBotCommandHandler, UserVoiceCommandHandler>();

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();
