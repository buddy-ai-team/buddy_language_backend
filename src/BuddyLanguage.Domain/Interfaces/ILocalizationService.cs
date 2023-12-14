using BuddyLanguage.Domain.Entities;
using BuddyLanguage.Domain.Enumerations;

namespace BuddyLanguage.Domain.Interfaces;

public interface ILocalizationService
{
    Task<string> GetText(LocalizableText text, Language preferredLanguage, CancellationToken cancellationToken);

    Task<byte[]> GetSpeech(LocalizableText text, Language preferredLanguage, CancellationToken cancellationToken);
}
