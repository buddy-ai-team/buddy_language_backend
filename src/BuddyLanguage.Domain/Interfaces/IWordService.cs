using BuddyLanguage.Domain.Entities;
using BuddyLanguage.Domain.Enumerations;

namespace BuddyLanguage.Domain.Services;

public interface IWordService
{
    Task<WordEntity> GetWordById(Guid id, CancellationToken cancellationToken);

    Task<IReadOnlyList<WordEntity>> GetWordsByAccountId(Guid userId, CancellationToken cancellationToken);

    Task<WordEntity> UpdateWordEntityById(
        Guid id,
        string word,
        string? translation,
        Language language,
        WordEntityStatus status,
        CancellationToken cancellationToken);

    Task<WordEntity> AddWord(
        Guid accountId,
        string word,
        string? translation,
        Language language,
        WordEntityStatus status,
        CancellationToken cancellationToken);
}
