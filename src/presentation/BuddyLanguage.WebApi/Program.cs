using BuddyLanguage.ChatGPTService;
using BuddyLanguage.Data.EntityFramework;
using BuddyLanguage.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using OpenAI.ChatGpt.AspNetCore.Models;
using OpenAI.ChatGpt.EntityFrameworkCore.Extensions;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();


// Add services to the container.

builder.Services.AddChatGptEntityFrameworkIntegration(
    options => options.UseSqlite("Data Source=chats.db")); // Или используем "myapp.db"???

builder.Services.AddScoped<IChatGPTService, ChatGPTService>(); 

// Definition of database file name and connection of it as a service
var dbPath = "myapp.db";
builder.Services.AddDbContext<AppDbContext>(
    options => options.UseSqlite($"Data Source={dbPath}"));

app.Run();