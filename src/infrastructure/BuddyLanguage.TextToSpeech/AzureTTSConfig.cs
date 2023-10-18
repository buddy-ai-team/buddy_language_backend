using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuddyLanguage.TextToSpeech
{
    public class AzureTTSConfig
    {
        [Required]
        public string ASPNETCORE_AZURE_SPEECH_KEY { get; set; }
        [Required]
        public string ASPNETCORE_AZURE_SPEECH_REGION { get; set; }
    }
}
