using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;

namespace BuddyLanguage.Telegram.Bot
{
	public abstract class ReceiverServiceBase<TUpdateHandler> : IReceiverService where TUpdateHandler : IUpdateHandler
	{
		private readonly ITelegramBotClient _botClient; // клиент Telegram бота, который предоставляет доступ к API Telegram
		private readonly IUpdateHandler _updateHandler; // обработчик обновлений, который будет использоваться для обработки поступающих обновлений
		private readonly ILogger<ReceiverServiceBase<TUpdateHandler>> _logger;

		internal ReceiverServiceBase(ITelegramBotClient botClient, TUpdateHandler updateHandler, ILogger<ReceiverServiceBase<TUpdateHandler>> logger)
		{
			_botClient = botClient;
			_updateHandler = updateHandler;
			_logger = logger;
		}

		public async Task ReceiveAsync(CancellationToken stoppingToken)
		{
			// разрешено обновление определенных типов 
			var receiverOptions = new ReceiverOptions()
			{
				AllowedUpdates = Array.Empty<UpdateType>(),
				ThrowPendingUpdates = true,
			};
			// получает инф о боте
			var me = await _botClient.GetMeAsync(stoppingToken);
			_logger.LogInformation("Start receiving updates for {BotName}", me.Username ?? "My Awesome Bot");
			// запуск обновлений
			await _botClient.ReceiveAsync(updateHandler: _updateHandler, receiverOptions: receiverOptions, cancellationToken: stoppingToken);
		}
	}
}
