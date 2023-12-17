using BuddyLanguage.Domain.Enumerations;
using BuddyLanguage.Domain.Interfaces;
using BuddyLanguage.Domain.Services;
using Telegram.Bot;
using Telegram.Bot.Types;
using User = BuddyLanguage.Domain.Entities.User;

namespace BuddyLanguage.TelegramBot.Commands;

public class ResetTopicCommand : BotTextCommandHandler
{
    private readonly ITelegramBotClient _botClient;
    private readonly UserService _userService;
    private readonly ILogger<ResetTopicCommand> _logger;
    private readonly BuddyService _buddyService;
    private readonly IChatGPTService _chatGPTService;

    public ResetTopicCommand(
        ITelegramBotClient botClient,
        UserService userService,
        ILogger<ResetTopicCommand> logger,
        BuddyService buddyService,
        IChatGPTService chatGPTService)
    {
        _botClient = botClient ?? throw new ArgumentNullException(nameof(botClient));
        _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _buddyService = buddyService ?? throw new ArgumentNullException(nameof(buddyService));
        _chatGPTService = chatGPTService ?? throw new ArgumentNullException(nameof(chatGPTService));
    }

    public override int Order => 0;

    public override string Command => "/reset";

    public override async Task HandleAsync(Update update, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(update);
        if (update.Message is { From: not null } message)
        {
            var telegramId = message.From.Id.ToString();
            User user = await _userService.GetUserByTelegramId(telegramId, cancellationToken);
            var nativeLanguage = user.UserPreferences.NativeLanguage;
            var sourceLanguage = Language.Russian;

            var textInNativeLanguage = await _chatGPTService.GetTextTranslatedIntoNativeLanguage(
                "Тема сброшена", sourceLanguage, nativeLanguage, cancellationToken);

            await _buddyService.ResetTopic(user, cancellationToken);
            await _botClient.SendTextMessageAsync(
                message.Chat.Id, textInNativeLanguage, cancellationToken: cancellationToken);
        }
    }
}
