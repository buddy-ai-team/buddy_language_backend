using System;
using System.Collections.Concurrent;
using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;

namespace BuddyLanguage.TelegramBot.Services;

public class TelegramUserRepositoryInCache
{
    private const string CorrectPassword = "112";
    private readonly IDistributedCache _cache;

    public TelegramUserRepositoryInCache(IDistributedCache cache)
    {
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
    }

    public void SetUserState(long userId, UserBotStates state)
    {
        var stateString = JsonSerializer.Serialize(state);
        _cache.SetString(userId.ToString(), stateString);
    }

    public UserBotStates GetUserState(long userId)
    {
        var stateString = _cache.GetString(userId.ToString());
        return stateString != null
            ? JsonSerializer.Deserialize<UserBotStates>(stateString)
            : UserBotStates.NotSet;
    }

    public void RemoveUserState(long userId)
    {
        _cache.Remove(userId.ToString());
    }

    public bool IsUserInState(long userId, UserBotStates state)
    {
        return GetUserState(userId) == state;
    }

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

    public bool IsAuthenticated(long userId)
    {
        return IsUserInState(userId, UserBotStates.Authenticated);
    }
}
