namespace BuddyLanguage.TelegramBot.Services;

public enum UserBotStates
{
    /// <summary>
    /// State is not set
    /// </summary>
    NotSet,

    /// <summary>
    /// Appears when user try to register
    /// </summary>
    AccessPassword,

    /// <summary>
    /// Appears when user is authenticated
    /// </summary>
    Authenticated,
}
