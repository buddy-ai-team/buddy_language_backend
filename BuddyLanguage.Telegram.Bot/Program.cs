using BuddyLanguage.Telegram.Bot;
using Telegram.Bot;
using Telegram.Bot.Polling;

IHost host = Host.CreateDefaultBuilder(args).ConfigureServices((context, services) =>
	{
		// Register Bot configuration
		services.Configure<BotConfiguration>(
			context.Configuration.GetSection(BotConfiguration.Configuration));

		services.AddHttpClient("telegram_bot_client")
				.AddTypedClient<ITelegramBotClient>((httpClient, sp) =>
				{
					BotConfiguration? botConfig = sp.GetRequiredService<IConfiguration>().GetSection(BotConfiguration.Configuration).Get<BotConfiguration>(); ;
					TelegramBotClientOptions options = new TelegramBotClientOptions(botConfig.BotToken);
					return new TelegramBotClient(options, httpClient);
				});

		services.AddScoped<UpdateHandler>();
		services.AddScoped<ReceiverService<IUpdateHandler>>();
		services.AddHostedService<PollingService<IReceiverService>>();
	})
	.Build();

await host.RunAsync();

public class BotConfiguration
{
	public static readonly string Configuration = "BotConfiguration";
	public string BotToken { get; set; } = "";
}
