using BuddyLanguage.Domain.Entities;
using BuddyLanguage.Domain.Enumerations;
using BuddyLanguage.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BuddyLanguage.Data.EntityFramework.Repositories
{
    public class WordEntityRepositoryEf : EfRepository<WordEntity>, IWordEntityRepository
    {
        public WordEntityRepositoryEf(AppDbContext dbContext)
            : base(dbContext)
        {
        }

        public async Task<IReadOnlyList<WordEntity>> GetWordsByUserId(Guid accountId, CancellationToken cancellationToken)
        {
            var words = await Entities.Where(x => x.UserId == accountId)
                .ToListAsync(cancellationToken);
            return words.AsReadOnly();
        }

        public async Task<WordEntity?> FindWordByName(
            Guid accountId, string word, CancellationToken cancellationToken)
        {
            return await Entities.Where(x => x.UserId == accountId)
                .SingleOrDefaultAsync(w => w.Word == word);
        }
    }
}
