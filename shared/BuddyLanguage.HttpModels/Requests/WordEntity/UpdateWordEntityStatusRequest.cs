using System.ComponentModel.DataAnnotations;
using BuddyLanguage.Domain.Enumerations;

namespace BuddyLanguage.HttpModels.Requests.WordEntity
{
    public class UpdateWordEntityStatusRequest
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        public WordEntityStatus Status { get; set; }
    }
}
