using BuddyLanguage.Domain.Entities;
using BuddyLanguage.Domain.Exceptions.User;
using BuddyLanguage.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BuddyLanguage.Data.EntityFramework.Repositories
{
    public class UserRepositoryEf : EfRepository<User>, IUserRepository
    {
        public UserRepositoryEf(AppDbContext dbContext)
            : base(dbContext)
        {
        }

        public override async Task<User> GetById(Guid id, CancellationToken cancellationToken)
        {
            return await Entities.Include(x => x.UserPreferences.AssistantRole)
                .FirstAsync(it => it.Id == id, cancellationToken);
        }

        public async Task<User> GetUserByTelegramId(string telegramId, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(telegramId);
            var user =
                await Entities
                .Include(x => x.UserPreferences.AssistantRole)
                .SingleOrDefaultAsync(
                    x => x.TelegramId == telegramId,
                    cancellationToken);
            if (user is null)
            {
                throw new UserNotFoundException("User with given telegramId not found");
            }

            return user;
        }

        public Task<User?> FindUserByTelegramId(
            string telegramId,
            CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(telegramId);
            return Entities
                .Include(x => x.UserPreferences.AssistantRole)
                .SingleOrDefaultAsync(
                x => x.TelegramId == telegramId, cancellationToken);
        }
    }
}
