using BuddyLanguage.Domain.Entities;
using BuddyLanguage.Domain.Enumerations;

namespace BuddyLanguage.Domain.Interfaces
{
    // Rating: https://huggingface.co/spaces/lmsys/chatbot-arena-leaderboard
    public interface IChatGPTService
    {
        /// <summary>
        /// Получает ответ на сообщение пользователя по заданной теме.
        /// </summary>
        /// <param name="userMessage">Сообщение пользователя.</param>
        /// <param name="userId">Идентификатор пользователя.</param>
        /// <param name="role">Роль пользователя</param>
        /// <param name="cancellation">Токен отмены для отмены операции.</param>
        /// <returns>Ответ на сообщение пользователя.</returns>
        Task<string> GetAnswerOnTopic(
            string userMessage, Guid userId, Role role, CancellationToken cancellation);

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

        Task<string> GetTextTranslatedIntoNativeLanguage(
            string text, Language sourceLanguage, Language nativeLanguage, CancellationToken cancellationToken);

        Task<TResult> GetStructuredAnswer<TResult>(
            string prompt, string userMessage, CancellationToken cancellationToken);

        Task ResetTopic(Guid userId, CancellationToken cancellationToken);
    }
}
