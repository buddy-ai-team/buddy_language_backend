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
    builder.Logging.ClearProviders();

    // https://github.com/dmitry-slabko/article-azure-logging-pub/blob/main/AzureLogging/Logging/Registration/HostBuilderExtensions.cs
    builder.Host.UseSerilog(ConfigureLogger, writeToProviders: false);

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

return;

static void ConfigureLogger(HostBuilderContext context, LoggerConfiguration config)
{
    // TODO: move to infrastructure
    config.Enrich.FromLogContext()
        .MinimumLevel.Debug()
        .WriteTo.Console();

    if (!context.HostingEnvironment.IsDevelopment())
    {
        config.WriteTo.Sentry(o =>
            {
                o.Dsn = context.Configuration["Sentry:Dsn"];
                o.MinimumBreadcrumbLevel = LogEventLevel.Debug;
                o.MinimumEventLevel = LogEventLevel.Error;
            })
            .WriteTo.Async(x =>
            {
                string azureLogFile = $@"D:\home\LogFiles\Application\{context.HostingEnvironment.ApplicationName}.txt";
                x.File(
                    azureLogFile,
                    shared: true,
                    flushToDiskInterval: TimeSpan.FromSeconds(1));
            });
    }
}
