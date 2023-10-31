using System.Collections.Concurrent;

namespace BuddyLanguage.TelegramBot.Services;

public class BotUserStateService
{
    private const string CorrectPassword = "112";

    private readonly ConcurrentDictionary<long, UserBotStates> _userStates = new ConcurrentDictionary<long, UserBotStates>();

    /// <summary>
    /// Sets the state for a specific user.
    /// </summary>
    /// <param name="userId">The user's ID.</param>
    /// <param name="state">The desired state.</param>
    public void SetUserState(long userId, UserBotStates state)
    {
        _userStates[userId] = state;
    }

    /// <summary>
    /// Gets the state for a specific user. If the user doesn't have a state, it will return NotSet.
    /// </summary>
    /// <param name="userId">The user's ID.</param>
    /// <returns>The user's state.</returns>
    public UserBotStates GetUserState(long userId)
    {
        return _userStates.TryGetValue(userId, out UserBotStates state) ? state : UserBotStates.NotSet;
    }

    /// <summary>
    /// Removes a user's state.
    /// </summary>
    /// <param name="userId">The user's ID.</param>
    public void RemoveUserState(long userId)
    {
        _userStates.TryRemove(userId, out _);
    }

    /// <summary>
    /// Checks if a user has a specific state.
    /// </summary>
    /// <param name="userId">The user's ID.</param>
    /// <param name="state">The state to check.</param>
    /// <returns>True if the user has the specified state, otherwise false.</returns>
    public bool IsUserInState(long userId, UserBotStates state)
    {
        return GetUserState(userId) == state;
    }

    /// <summary>
    /// Verifies if the provided password is correct and updates the user's state accordingly.
    /// </summary>
    /// <param name="userId">The user's ID.</param>
    /// <param name="password">The password to check.</param>
    /// <returns>True if the password is correct, otherwise false.</returns>
    public bool VerifyPasswordAndUpdateState(long userId, string password)
    {
        ArgumentNullException.ThrowIfNull(password);
        if (password == CorrectPassword)
        {
            SetUserState(userId, UserBotStates.Authenticated);
            return true;
        }

        return false;
    }

    /// <summary>
    /// Checks if a user is authenticated.
    /// </summary>
    /// <param name="userId">The user's ID.</param>
    /// <returns>True if the user is authenticated, otherwise false.</returns>
    public bool IsAuthenticated(long userId)
    {
        return IsUserInState(userId, UserBotStates.Authenticated);
    }
}
