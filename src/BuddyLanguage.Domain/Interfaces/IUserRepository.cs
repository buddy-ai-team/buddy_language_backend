using BuddyLanguage.Domain.Entities;
using System.Security.Principal;

namespace BuddyLanguage.Domain.Interfaces
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User> GetUserByTelegramId(string telegramId, CancellationToken cancellationToken);
    }
}
