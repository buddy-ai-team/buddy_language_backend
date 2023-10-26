using BuddyLanguage.ChatGPTServiceLib;
using BuddyLanguage.Data.EntityFramework;
using BuddyLanguage.Data.EntityFramework.Repositories;
using BuddyLanguage.DependencyInjection;
using BuddyLanguage.Domain.Interfaces;
using BuddyLanguage.Domain.Services;
using Microsoft.EntityFrameworkCore;
using OpenAI.ChatGpt.EntityFrameworkCore.Extensions;
using BuddyLanguage.OpenAIWhisperSpeechRecognitionService;
using BuddyLanguage.TextToSpeech;
using BuddyLanguage.WebApi.Filters;
using OpenAI.Extensions;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

//Services

builder.Services.AddInfrastructureServices(builder.Configuration);

// Подключение фильтров
builder.Services.AddControllers(options =>
{
    options.Filters.Add<CentralizedExceptionHandlingFilter>(order: 1);
});

var app = builder.Build();

app.MapControllers();

app.Run();