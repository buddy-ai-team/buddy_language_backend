using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuddyLanguage.TextToSpeech
{
    // ReSharper disable once InconsistentNaming
    public class AzureTTSConfig
    {
        [Required]
        public string SpeechKey { get; set; } = null!;

        [Required]
        public string SpeechRegion { get; set; } = null!;
    }
}
