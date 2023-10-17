using BuddyLanguage.ChatGPTService;
using BuddyLanguage.Data.EntityFramework;
using BuddyLanguage.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using OpenAI.ChatGpt.AspNetCore.Models;
using OpenAI.ChatGpt.EntityFrameworkCore.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Definition of database file name and connection of it as a service
var dbPath = "myapp.db";
builder.Services.AddDbContext<AppDbContext>(
    options => options.UseSqlite($"Data Source={dbPath}"));


builder.Services.AddChatGptEntityFrameworkIntegration(
    options => options.UseSqlite($"Data Source={dbPath}"));

builder.Services.AddScoped<IChatGPTService, ChatGPTService>(); 

var app = builder.Build();

app.Run();