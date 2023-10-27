using BuddyLanguage.Domain.Enumerations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuddyLanguage.HttpModels.Requests.User
{
    public class UpdateUserRequest
    {
        [Required]
        public Guid Id { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required] 
        public string LastName { get;set; }
        [Required]
        public string TelegramId { get; set; }
    }
}
