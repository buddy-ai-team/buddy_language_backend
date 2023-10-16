namespace BuddyLanguage.Telegram.Bot
{
	// интерфейс для получения обновления
	public interface IReceiverService
	{
		Task ReceiveAsync(CancellationToken stoppingToken);
	}
}
