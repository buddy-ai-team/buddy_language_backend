using System.ComponentModel.DataAnnotations;
using BuddyLanguage.Domain.Enumerations;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

namespace BuddyLanguage.HttpModels.Requests.WordEntity
{
    public class UpdateWordEntityRequest
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        public string Word { get; set; }

        [Required]
        public Language Language { get; set; }

        [Required]
        public WordEntityStatus Status { get; set; }
    }
}
