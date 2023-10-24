using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuddyLanguage.Domain.Interfaces
{
    public interface IChatGPTService
    {
        /// <summary>
        /// Получает ответ на сообщение пользователя по заданной теме.
        /// </summary>
        /// <param name="userMessage">Сообщение пользователя.</param>
        /// <param name="userId">Идентификатор пользователя.</param>
        /// <param name="cancellation">Токен отмены для отмены операции.</param>
        /// <returns>Ответ на сообщение пользователя.</returns>
        Task<string> GetAnswerOnTopic(string userMessage, Guid userId, CancellationToken cancellation);

        /// <summary>
        /// Получает ответ на сообщение пользователя.
        /// </summary>
        /// <param name="userMessage">Сообщение пользователя.</param>
        /// <param name="cancellationToken">Токен отмены для отмены операции.</param>
        /// <returns>Ответ на сообщение пользователя.</returns>
        Task<string> GetAnswer(string userMessage, CancellationToken cancellationToken);

        /// <summary>
        /// Получает ответ на сообщение пользователя, используя заданный промпт.
        /// </summary>
        /// <param name="prompt">Промпт для генерации ответа.</param>
        /// <param name="userMessage">Сообщение пользователя.</param>
        /// <param name="cancellationToken">Токен отмены для отмены операции.</param>
        /// <returns>Ответ на сообщение пользователя.</returns>
        Task<string> GetAnswer(string prompt, string userMessage, CancellationToken cancellationToken);
    }
}
