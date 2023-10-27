using BuddyLanguage.TelegramBot;
using BuddyLanguage.TelegramBot.Commands;
using Telegram.Bot;

var builder = WebApplication.CreateBuilder(args);

var token = builder.Configuration["BotConfiguration:BotToken"];
if(string.IsNullOrEmpty(token))
{
    throw new InvalidOperationException("Telegram bot token is not set");
}
builder.Services.AddSingleton<ITelegramBotClient>(new TelegramBotClient(token));
builder.Services.AddHostedService<TelegramBotUpdatesListener>();

builder.Services.AddSingleton<BotCommandHandler, StartCommandHandler>();
builder.Services.AddSingleton<BotCommandHandler, SetBaseLanguageCommandHandler>();
builder.Services.AddSingleton<BotCommandHandler, UserVoiceCommandHandler>();
builder.Services.AddSingleton<BotCommandHandler, UserVoiceCommandHandler>();

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();