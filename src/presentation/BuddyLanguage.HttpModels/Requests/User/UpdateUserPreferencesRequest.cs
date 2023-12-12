using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BuddyLanguage.Domain.Enumerations;

namespace BuddyLanguage.HttpModels.Requests.User
{
    public class UpdateUserPreferencesRequest
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        [Range(0, 25)]
        public Language NativeLanguage { get; set; }

        [Required]
        [Range(0, 25)]
        public Language TargetLanguage { get; set; }

        [Required]
        [Range(0, 4)]
        public TtsSpeed SelectedSpeed { get; set; }

        [Required]
        [Range(0, 1)]
        public Voice SelectedVoice { get; set; }

        [Required]
        public Guid AssistantRoleId { get; set; }
    }
}
