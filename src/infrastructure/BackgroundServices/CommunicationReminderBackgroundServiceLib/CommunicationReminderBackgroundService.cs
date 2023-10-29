using System.Data;
using BuddyLanguage.Domain.Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CommunicationReminderBackgroundServiceLib
{
    public class CommunicationReminderBackgroundService : BackgroundService
    {
        private readonly ILogger<CommunicationReminderBackgroundService> _logger;
        private readonly IChatGPTService _chatGPTSevice;
        private readonly ITelegramBotClient _botClient;
        private readonly int _reminderIntervalHours = 24; //количество часов отсутствия пользователя в приложении
        private readonly string _prompt = "Пользователь долго не появляется в приложении нужно его вдохновить " +
                                          "на возвращение, продолжение общения и дальнейшее изучение языка. " +
                                          "Сделай это в веселой шуточной форме.";

        public CommunicationReminderBackgroundService(ILogger<CommunicationReminderBackgroundService> logger, IChatGPTService chatGPTService, ITelegramBotClient botClient)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _chatGPTSevice = chatGPTService ?? throw new ArgumentNullException(nameof(_chatGPTSevice));
            _botClient = botClient ?? throw new ArgumentNullException(nameof(botClient));
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested) //пока не запрошена отмена задачи
            {
                await CheckAndSendReminder(cancellationToken); //метод проверяет время последнего сообщения и отправляет напоминание

                await Task.Delay(TimeSpan.FromHours(4)); //делаем это каждые 4 часа
            }
        }

        private async Task CheckAndSendReminder(CancellationToken cancellationToken)
        {
            //здесь не уверена, что правильно получаю данные. Даже скорее почти уверена, что неправильно...
            var updates = await _botClient.GetUpdatesAsync();
            var update = updates.Last();
            var chatId = update.Message.Chat.Id;
            var lastMessageDate = update.Message.Date;

            var currentTime = DateTime.Now;
            var afterLastMessageIntervalHours = (currentTime - lastMessageDate).TotalHours; //считаем сколько часов прошло после последнего сообщения
            if (afterLastMessageIntervalHours >= _reminderIntervalHours) //если прошло 48 часов и больше
            {
                var reminder = await _chatGPTSevice.GetAnswer(_prompt, cancellationToken); //отправляем чату GPT запрос на напоминание
                await _botClient.SendTextMessageAsync(//отправляем напоминание пользователю
                    chatId: chatId,
                    text: reminder);
            }
        }
    }
}
