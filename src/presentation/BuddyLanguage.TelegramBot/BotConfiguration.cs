using System.ComponentModel.DataAnnotations;

namespace BuddyLanguage.TelegramBot;

public class BotConfiguration
{
    [Required]
    public string Token { get; set; } = null!;

    [Required]
    [Url]
    public string WebHookHost { get; set; } = null!;
}
