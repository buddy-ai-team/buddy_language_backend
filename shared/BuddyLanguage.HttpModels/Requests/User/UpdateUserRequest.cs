using System.ComponentModel.DataAnnotations;
#pragma warning disable CS8618  // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

namespace BuddyLanguage.HttpModels.Requests.User
{
    public class UpdateUserRequest
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required] 
        public string LastName { get; set; }

        [Required]
        public string TelegramId { get; set; }
    }
}
