using System.ComponentModel.DataAnnotations;

namespace BuddyLanguage.AzureServices
{
    // ReSharper disable once InconsistentNaming
    public class AzureConfig
    {
        [Required]
        public string SpeechKey { get; set; } = null!;

        [Required]
        public string SpeechRegion { get; set; } = null!;
    }
}
