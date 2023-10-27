using BuddyLanguage.Domain.Entities;
using BuddyLanguage.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace BuddyLanguage.Data.EntityFramework.Repositories
{
    public class UserRepositoryEf : EfRepository<User>, IUserRepository
    {
        public UserRepositoryEf(AppDbContext dbContext) : base(dbContext)
        {
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
