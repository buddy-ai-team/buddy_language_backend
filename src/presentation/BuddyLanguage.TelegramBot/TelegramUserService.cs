using BuddyLanguage.TelegramBot.Services;

namespace BuddyLanguage.TelegramBot;

public class TelegramUserService
{
    private readonly TelegramUserRepositoryInCache _telegramUserRepository;

    public TelegramUserService(TelegramUserRepositoryInCache telegramUserRepository)
    {
        _telegramUserRepository = telegramUserRepository;
    }

    public UserBotStates State { get; set; }

    public bool Authenticated { get; set; }
}
