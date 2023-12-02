namespace BuddyLanguage.TelegramBot.Models;

public record TelegramMessageBaseInfo(
    string UserId,
    long ChatId,
    string FirstName,
    string? LastName);
