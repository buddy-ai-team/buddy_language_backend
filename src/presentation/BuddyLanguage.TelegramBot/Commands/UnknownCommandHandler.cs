using Telegram.Bot;
using Telegram.Bot.Types;

namespace BuddyLanguage.TelegramBot.Commands
{
    public class UnknownCommandHandler : IBotCommandHandler
    {
        private readonly ITelegramBotClient _botClient;
        private readonly ILogger<UnknownCommandHandler> _logger;

        public UnknownCommandHandler(
            ITelegramBotClient botClient,
            ILogger<UnknownCommandHandler> logger)
        {
            _botClient = botClient ?? throw new ArgumentNullException(nameof(botClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public string? Command => null;

        public async Task HandleAsync(Update update, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Unknown command received");
            if (update.Message != null)
            {
                await _botClient.SendTextMessageAsync(
                    chatId: update.Message.Chat.Id,
                    text: "Пока что умею отвечать только на голосовые собощения на английском языке.",
                    cancellationToken: cancellationToken);
            }
        }

        public bool CanHandleCommand(Update update)
        {
            return false;
        }
    }
}
