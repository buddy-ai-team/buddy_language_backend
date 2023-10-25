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
    public class WordEntityRepositoryEf : EfRepository<WordEntity>, IWordEntityRepository
    {
        public WordEntityRepositoryEf(AppDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<IReadOnlyList<WordEntity>> GetWordsByAccountId(Guid accountId, CancellationToken cancellationToken)
        {
            var words = await Entities.Where(x => x.AccountId == accountId).ToListAsync(cancellationToken);
            return words.AsReadOnly();
        }
    }
}
