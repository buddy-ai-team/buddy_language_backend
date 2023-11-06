using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuddyLanguage.Domain.Interfaces
{
    public interface IMessageSendingService
    {
        /// <summary>
        /// Метод проверяет время последнего сообщения пользователя и отправляет напоминание,
        /// если пользователь отсутствовал больше установленного времени
        /// </summary>
        /// <param name="reminderIntervalHours">Количество часов отсутствия пользователя в приложении</param>
        /// <param name="cancellationToken">Токен отмены для отмены операции.</param>
        /// <returns><see cref="Task"/> Представляет асинхронную операцию.</returns>
        Task CheckAndSendReminder(int reminderIntervalHours, CancellationToken cancellationToken);
    }
}
