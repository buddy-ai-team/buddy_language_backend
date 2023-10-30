using System.ComponentModel.DataAnnotations;

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
