using BuddyLanguage.Domain.Entities;

namespace BuddyLanguage.Domain.Interfaces
{
    public interface IWordEntityRepository : IRepository<WordEntity>
    {
        Task<IReadOnlyList<WordEntity>> GetWordsByAccountId(Guid accountId, CancellationToken cancellationToken);
    }
}
