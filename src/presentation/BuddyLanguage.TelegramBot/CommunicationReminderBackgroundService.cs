using BuddyLanguage.TelegramBot.Services;

namespace BuddyLanguage.TelegramBot
{
    public class CommunicationReminderBackgroundService : BackgroundService
    {
        private readonly TelegramMessageSendingService _telegramMessageSendingService;

        public CommunicationReminderBackgroundService(TelegramMessageSendingService telegramMessageSendingService)
        {
            _telegramMessageSendingService = telegramMessageSendingService ?? throw new ArgumentNullException(nameof(telegramMessageSendingService)); 
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested) 
            {
                await _telegramMessageSendingService.CheckAndSendReminder(24, cancellationToken);

                await Task.Delay(TimeSpan.FromMinutes(15));
            }
        }
    }
}
