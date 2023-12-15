using BuddyLanguage.ChatGPTServiceLib;
using BuddyLanguage.Domain.Interfaces;
using BuddyLanguage.Domain.Services;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace BuddyLanguage.TelegramBot.Commands
{
    public class UnknownCommandHandler : IBotCommandHandler
    {
        private readonly ITelegramBotClient _botClient;
        private readonly IChatGPTService _chatGPTService;
        private readonly UserService _userService;
        private readonly ILogger<UnknownCommandHandler> _logger;

        public UnknownCommandHandler(
            ITelegramBotClient botClient,
            IChatGPTService chatGPTService,
            UserService userService,
            ILogger<UnknownCommandHandler> logger)
        {
            _botClient = botClient ?? throw new ArgumentNullException(nameof(botClient));
            _chatGPTService = chatGPTService ?? throw new ArgumentNullException(nameof(chatGPTService));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public int Order => int.MaxValue;

        public string? Command => null;

        public async Task HandleAsync(Update update, CancellationToken cancellationToken)
        {
            var telegramUserId = update.Message?.From?.Id.ToString();
            if (telegramUserId is null)
            {
                _logger.LogError("Can`t get telegram user id from update message! {@Update}", update);
                return;
            }

            var user = await _userService.GetUserByTelegramId(telegramUserId, cancellationToken);

            var nativeLanguage = user.UserPreferences.NativeLanguage;
            var targetLanguage = user.UserPreferences.TargetLanguage;

            _logger.LogInformation("Unknown command received");
            var text = "Пока что умею отвечать только на голосовые собощения на изучаемом языке.";
            var textInNativeLanguage = await _chatGPTService.GetTextTranslatedIntoNativeLanguage(
                text, nativeLanguage, targetLanguage, cancellationToken); 
            if (update.Message != null)
            {
                await _botClient.SendTextMessageAsync(
                    chatId: update.Message.Chat.Id,
                    text: textInNativeLanguage,
                    cancellationToken: cancellationToken);
            }
        }

        public bool CanHandleCommand(Update update) => true;
    }
}
