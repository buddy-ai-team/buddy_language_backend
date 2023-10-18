using BuddyLanguage.Data.EntityFramework;
using BuddyLanguage.Domain.Interfaces;
using BuddyLanguage.OpenAIWhisperSpeechRecognitionService;
using Microsoft.EntityFrameworkCore;
using OpenAI.Extensions;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var dbPath = "myapp.db";
builder.Services.AddDbContext<AppDbContext>(
    options => options.UseSqlite($"Data Source={dbPath}"));

builder.Services.AddOpenAIService
    (settings =>
    {
        settings.ApiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY")
                          ?? throw new InvalidOperationException
                          ("OPENAI_API_KEY environment variable is not set"); ;
    });

builder.Services.AddScoped<ISpeechRecognitionService, WhisperSpeechRecognitionService>();

app.Run();