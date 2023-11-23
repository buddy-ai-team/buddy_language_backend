using BuddyLanguage.Infrastructure;
using BuddyLanguage.WebApi.Extensions;
using BuddyLanguage.WebApi.Filters;
using Serilog;
using Serilog.Events;

var builder = WebApplication.CreateBuilder(args);

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

try
{
    builder.Host.UseSerilog((_, config) => config.ReadFrom.Configuration(builder.Configuration).WriteTo.Console());

    builder.WebHost.UseSentry();

    //WebApi services
    builder.Services.AddWebApiServices(builder.Configuration);

    //Domain and Infrastructure services
    builder.Services.AddApplicationServices(builder.Configuration);

    var app = builder.Build();

    app.UseWebApi();
    app.MapHealthChecks("/buddy_health")
        .ShortCircuit();

    app.Run();
}
catch (Exception ex) when (ex is not HostAbortedException)
{
    Log.Fatal(ex, "Unhandled exception on server startup");
}
finally
{
    Log.Information("Shut down complete");
    Log.CloseAndFlush();
}
