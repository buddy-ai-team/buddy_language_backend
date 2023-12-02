using BuddyLanguage.Domain.Enumerations;

namespace BuddyLanguage.Domain.Entities;

#pragma warning disable CS8618 

public class WordEntity : IEntity
{
    private string _word;
    private string _translation;

    public WordEntity(Guid id, Guid accountId, string word, string translation, Language language, WordEntityStatus wordStatus)
    {
        Id = id;
        UserId = accountId;
        _word = word;
        _translation = translation;
        Language = language;
        WordStatus = wordStatus;    
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

    public string Translation
    {
        get => _translation;
        set
        {
            ArgumentException.ThrowIfNullOrEmpty(value);
            _translation = value;
        }
    }
}
