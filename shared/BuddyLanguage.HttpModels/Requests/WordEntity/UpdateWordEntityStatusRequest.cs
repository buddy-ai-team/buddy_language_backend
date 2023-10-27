using BuddyLanguage.Domain.Enumerations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
