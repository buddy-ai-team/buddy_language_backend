using System.ComponentModel.DataAnnotations;
using BuddyLanguage.Domain.Enumerations;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

namespace BuddyLanguage.HttpModels.Requests.WordEntity
{
    public class AddWordEntityRequest
    {
        [Required]
        public string Word { get; set; }

        [Required]
        public Guid AccountId { get; init; }

        [Required]
        public WordEntityStatus Status { get; set; }
    }
}
