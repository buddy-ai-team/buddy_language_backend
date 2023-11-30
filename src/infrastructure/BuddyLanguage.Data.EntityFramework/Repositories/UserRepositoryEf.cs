using BuddyLanguage.Domain.Entities;
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
            return await Entities.Include(x => x.AssistantRole)
                                 .FirstAsync(it => it.Id == id, cancellationToken);              
        }

        public Task<User> GetUserByTelegramId(string telegramId, CancellationToken cancellationToken)
        {
            if (telegramId is null)
            {
                throw new ArgumentNullException(nameof(telegramId));
            }

            return Entities.SingleAsync(x => x.TelegramId == telegramId, cancellationToken);
        }
    }
}
