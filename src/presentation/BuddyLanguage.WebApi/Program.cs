using BuddyLanguage.ChatGPTService;
using BuddyLanguage.Data.EntityFramework;
using BuddyLanguage.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using OpenAI.ChatGpt.EntityFrameworkCore.Extensions;
using BuddyLanguage.OpenAIWhisperSpeechRecognitionService;
using BuddyLanguage.TextToSpeech;
using OpenAI.Extensions;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

//Services
//Azure TTS
builder.Services.AddOptions<AzureTTSConfig>()
    .BindConfiguration("AzureTTSConfig")
    .ValidateDataAnnotations()
    .ValidateOnStart();

// Definition of database file name and connection of it as a service
builder.Services.AddOptions<NpgsqlConnectionStringOptions>()
    .BindConfiguration("NpgsqlConnectionStringOptions")
    .ValidateDataAnnotations()
    .ValidateOnStart();

var config = builder.Configuration
   .GetSection("NpgsqlConnectionStringOptions")
   .Get<NpgsqlConnectionStringOptions>();

builder.Services.AddDbContext<AppDbContext>(
    options => options.UseNpgsql(config.ConnectionString)
);

builder.Services.AddChatGptEntityFrameworkIntegration(
        op => op.UseNpgsql(config.ConnectionString)
);

builder.Services.AddScoped<IChatGPTService, ChatGPTService>(); 

builder.Services.AddOpenAIService
    (settings =>
    {
        settings.ApiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY")
                          ?? throw new InvalidOperationException
                          ("OPENAI_API_KEY environment variable is not set");
    });

builder.Services.AddScoped<ISpeechRecognitionService, WhisperSpeechRecognitionService>();

var app = builder.Build();

app.Run();