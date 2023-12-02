using BuddyLanguage.Domain.Entities;

namespace BuddyLanguage.Domain.Interfaces
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User> GetUserByTelegramId(string telegramId, CancellationToken cancellationToken);

        Task<User?> FindUserByTelegramId(string telegramId, CancellationToken cancellationToken);
    }
}
