﻿namespace BuddyLanguage.Telegram.Bot
{
	public abstract class PollingService<TReceiverService> : BackgroundService
		where TReceiverService : IReceiverService
	{
		// проверяет обновления Telegram-бот, учитывая обработку исключений и регистрацию ошибок
		private readonly IServiceProvider _serviceProvider;
		private readonly ILogger _logger;

		internal PollingService(IServiceProvider serviceProvider, ILogger<PollingService<TReceiverService>> logger)
		{
			_serviceProvider = serviceProvider;
			_logger = logger;
		}
		// Выводит сообщение, что метод запустился + вызывает метод DoWork, который содержит основную логику
		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			_logger.LogInformation("Starting polling service");

			await DoWork(stoppingToken);
		}

		private async Task DoWork(CancellationToken stoppingToken)
		{
			while(!stoppingToken.IsCancellationRequested)
			{
				try
				{
					using var scope = _serviceProvider.CreateScope();
					var receiver = scope.ServiceProvider.GetRequiredService<TReceiverService>();
					await receiver.ReceiveAsync(stoppingToken);
				}
				catch (Exception ex) 
				{
					_logger.LogError("Polling failed with exception: {Exception}", ex);
					// Dыполнение приостанавливается на 5 секунд, для обработки ошибок и предотвращения быстрого повторного запроса в случае проблем.
					await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
				}
			}
		}

	}
}
