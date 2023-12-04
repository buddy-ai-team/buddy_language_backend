using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuddyLanguage.HttpModels.Requests.User
{
    public class UpdateUserPreferencesRequest
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        public int NativeLanguage { get; set; }

        [Required]
        public int TargetLanguage { get; set; }

        [Required]
        public int SelectedSpeed { get; set; }

        [Required]
        public int SelectedVoice { get; set; }

        [Required]
        public Guid AssistantRoleId { get; set; }
    }
}
