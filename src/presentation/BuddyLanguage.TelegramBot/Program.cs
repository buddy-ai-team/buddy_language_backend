using BuddyLanguage.Infrastructure;
using BuddyLanguage.TelegramBot;
using BuddyLanguage.TelegramBot.Commands;
using BuddyLanguage.TelegramBot.Services;
using Serilog;
using Serilog.Events;
using Telegram.Bot;

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

    builder.Services.AddApplicationServices(builder.Configuration);

    var token = builder.Configuration.GetRequiredValue("BotConfiguration:Token");

    builder.Services.AddSingleton(new TelegramBotClientOptions(token));

    // TODO Implement Polly after update to .NET 8: https://github.com/dotnet/docs/blob/main/docs/core/resilience/http-resilience.md
    builder.Services.AddHttpClient("TelegramBotClient");
    builder.Services.AddSingleton<ITelegramBotClient>(provider =>
    {
        IHttpClientFactory factory = provider.GetRequiredService<IHttpClientFactory>();
        HttpClient client = factory.CreateClient("TelegramBotClient");
        return new TelegramBotClient(token, client);
    });

    builder.Services.AddSingleton<TelegramUserRepository>();

    builder.Services.AddHostedService<TelegramBotUpdatesListener>();

    builder.Services.Scan(scan => scan
        .FromAssemblyOf<StartCommandHandler>()
        .AddClasses(classes => classes.AssignableTo<IBotCommandHandler>())
        .AsImplementedInterfaces()
        .WithScopedLifetime());

    var app = builder.Build();

    app.MapGet("/", () => $"Ver: {ReflectionHelper.GetBuildDate():s}");

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
