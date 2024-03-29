﻿using System.ComponentModel.DataAnnotations;

namespace BuddyLanguage.TelegramBot.Configurations;

public class BotConfiguration
{
    [Required]
    public string Token { get; set; } = null!;

    [Required]
    [Url]
    public string WebHookHost { get; set; } = null!;

    [Required]
    public string WebHookSecret { get; set; } = null!;
}
