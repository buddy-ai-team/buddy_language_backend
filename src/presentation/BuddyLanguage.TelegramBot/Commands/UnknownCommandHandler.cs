using Telegram.Bot;
using Telegram.Bot.Types;

namespace BuddyLanguage.TelegramBot.Commands
{
    public class UnknownCommandHandler : IBotCommandHandler
    {
        private readonly ITelegramBotClient _botClient;

        public UnknownCommandHandler(ITelegramBotClient botClient)
        {
            _botClient = botClient ?? throw new ArgumentNullException(nameof(botClient));
        }

        public string? Command => null;

        public async Task HandleAsync(Update update, CancellationToken cancellationToken)
        {
            if (update.Message != null)
            {
                await _botClient.SendTextMessageAsync(
                    chatId: update.Message.Chat.Id,
                    text: "Извините, данная команда не распознана.",
                    cancellationToken: cancellationToken);
            }
        }

        public bool CanHandleCommand(Update update)
        {
            return false;
        }
    }
}
