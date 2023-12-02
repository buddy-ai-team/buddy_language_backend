using System.Transactions;
using BuddyLanguage.Domain.Enumerations;

namespace BuddyLanguage.Domain.Entities;

#pragma warning disable CS8618 

public class WordEntity : IEntity
{
    private string _word;

    public WordEntity(Guid id, Guid accountId, string word, Language language, WordEntityStatus wordStatus, string? translation = "")
    {
        Id = id;
        UserId = accountId;
        _word = word;
        Language = language;
        WordStatus = wordStatus;
        Translation = translation;
    }

    protected WordEntity()
    {
    }

    public Guid Id { get; init; }

    public Language Language { get; set; }

    public WordEntityStatus WordStatus { get; set; }

    //External Key To User.Id
    public Guid UserId { get; init; }

    public User User { get; set; }

    public string Word
    {
        get => _word;
        set
        {
            ArgumentException.ThrowIfNullOrEmpty(value);
            _word = value;
        }
    }

    public string? Translation { get; set; }
}
