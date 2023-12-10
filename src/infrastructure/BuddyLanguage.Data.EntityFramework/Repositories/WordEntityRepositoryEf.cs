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

        public async Task<int> GetNumberWordsLearned(Guid accountId, CancellationToken cancellationToken)
        {
            var words = await GetWordsByUserId(accountId, cancellationToken);
            var learnedWords = words.Count(x => x.WordStatus == WordEntityStatus.Learned);
            return learnedWords;
        }

        public async Task<int> GetNumberWordsLearning(Guid accountId, CancellationToken cancellationToken)
        {
            var words = await GetWordsByUserId(accountId, cancellationToken);
            var learningdWords = words.Count(x => x.WordStatus == WordEntityStatus.Learning);
            return learningdWords;
        }
    }
}
