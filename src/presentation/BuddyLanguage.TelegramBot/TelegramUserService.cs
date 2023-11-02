using BuddyLanguage.TelegramBot.Services;

namespace BuddyLanguage.TelegramBot;

public class TelegramUserService
{
    private readonly TelegramUserRepository _telegramUserRepository;

    public TelegramUserService(TelegramUserRepository telegramUserRepository)
    {
        _telegramUserRepository = telegramUserRepository;
    }

    public UserBotStates State { get; set; }

    public bool Authenticated { get; set; }
}
