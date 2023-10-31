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
        o.Dsn = "https://70228ed8115b87d41a3cf0c17896d3bd@o4506146415837184.ingest.sentry.io/4506146476654592";
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
