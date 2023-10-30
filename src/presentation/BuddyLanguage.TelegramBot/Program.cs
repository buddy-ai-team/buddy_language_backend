using BuddyLanguage.Infrastructure;
using BuddyLanguage.TelegramBot;
using BuddyLanguage.TelegramBot.Commands;
using Telegram.Bot;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplicationServices(builder.Configuration);

var token = builder.Configuration["BotConfiguration:BotToken"];
if (string.IsNullOrWhiteSpace(token))
{
    throw new InvalidOperationException("Telegram bot token is not set");
}

builder.Services.AddSingleton<ITelegramBotClient>(new TelegramBotClient(token));
builder.Services.AddHostedService<TelegramBotUpdatesListener>();

builder.Services.AddScoped<IBotCommandHandler, StartCommandHandler>();
builder.Services.AddScoped<IBotCommandHandler, UnknownCommandHandler>();
builder.Services.AddScoped<IBotCommandHandler, UserVoiceCommandHandler>();
builder.Services.AddScoped<IBotCommandHandler, UserVoiceCommandHandler>();

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();
