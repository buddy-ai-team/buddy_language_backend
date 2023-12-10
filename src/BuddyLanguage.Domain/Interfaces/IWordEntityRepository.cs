using BuddyLanguage.Domain.Entities;

namespace BuddyLanguage.Domain.Interfaces
{
    public interface IWordEntityRepository : IRepository<WordEntity>
    {
        Task<IReadOnlyList<WordEntity>> GetWordsByUserId(Guid accountId, CancellationToken cancellationToken);

        Task<int> GetNumberWordsLearned(Guid accountId, CancellationToken cancellationToken);

        Task<int> GetNumberWordsLearning(Guid accountId, CancellationToken cancellationToken);
    }
}
