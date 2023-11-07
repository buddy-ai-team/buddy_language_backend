namespace BuddyLanguage.Domain.Entities;

public interface ILanguage : IEntity
{
    /// <summary>
    /// Gets the language name. In native language.
    /// </summary>
    string Name { get; init; }

    /// <summary>
    /// Gets the language name. In English.
    /// </summary>
    string NameEn { get; init; }

    /// <summary>
    /// Gets the ISO 639-1 language code.
    /// </summary>
    /// <remarks>
    /// It should be ValueObject in future.
    /// </remarks>
    string Code { get; init; }

    bool TextContainsThisLanguage(string text);
}
