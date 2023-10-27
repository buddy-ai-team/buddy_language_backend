using BuddyLanguage.Domain.Enumerations;

namespace BuddyLanguage.HttpModels.Responses.WordEntity
{
    public record WordEntityResponse(Guid Id, Guid AccountId, string Word, WordEntityStatus Status);
}
