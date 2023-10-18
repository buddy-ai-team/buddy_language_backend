using BuddyLanguage.Data.EntityFramework;
using BuddyLanguage.TextToSpeech;
using Microsoft.EntityFrameworkCore;

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

var app = builder.Build();

app.Run();