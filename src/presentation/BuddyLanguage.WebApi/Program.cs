using System;
using BuddyLanguage.Infrastructure;
using BuddyLanguage.WebApi.Filters;
using Sentry.AspNetCore;
using Serilog;
using Serilog.Events;

var builder = WebApplication.CreateBuilder(args);

//Swagger Setup
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Domain and Infrastructure services
builder.Services.AddApplicationServices(builder.Configuration);

//Filters
builder.Services.AddControllers(options =>
{
    options.Filters.Add<CentralizedExceptionHandlingFilter>(order: 1);
});

// Setup Serilog and Sentry
Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .MinimumLevel.Debug()
    .WriteTo.Console()
    .WriteTo.Sentry(o =>
    {
        o.Dsn = builder.Configuration["Sentry:Dsn"];
        o.MinimumBreadcrumbLevel = LogEventLevel.Debug;
        o.MinimumEventLevel = LogEventLevel.Error;
    })
    .CreateLogger();

builder.Host.UseSerilog();

var app = builder.Build();

//Swagger Build
app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

app.Run();
