using BuddyLanguage.ChatGPTService;
using BuddyLanguage.Data.EntityFramework;
using BuddyLanguage.Data.EntityFramework.Repositories;
using BuddyLanguage.Domain.Interfaces;
using BuddyLanguage.Domain.Services;
using Microsoft.EntityFrameworkCore;
using OpenAI.ChatGpt.EntityFrameworkCore.Extensions;
using BuddyLanguage.OpenAIWhisperSpeechRecognitionService;
using BuddyLanguage.TextToSpeech;
using BuddyLanguage.WebApi.Filters;
using OpenAI.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

//Azure TTS
builder.Services.AddOptions<AzureTTSConfig>()
    .BindConfiguration("AzureTTSConfig")
    .ValidateDataAnnotations()
    .ValidateOnStart();

// Definition of database file name and connection of it as a service
var dbPath = "myapp.db";
builder.Services.AddDbContext<AppDbContext>(
    options => options.UseSqlite($"Data Source={dbPath}"));

// Подключение репозитория для работы с Ролями
builder.Services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));
builder.Services.AddScoped<IRoleRepository, RoleRepositoryEf>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWorkEf>();
builder.Services.AddScoped<RoleService>();

// Подключение фильтров
builder.Services.AddControllers(options =>
{
    options.Filters.Add<CentralizedExceptionHandlingFilter>(order: 1);
});


builder.Services.AddChatGptEntityFrameworkIntegration(
    options => options.UseSqlite($"Data Source={dbPath}"));

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

app.MapControllers();

app.Run();