using Telegram.Bot;
using Telegram.Bot.Types;

namespace BuddyLanguage.TelegramBot.Commands
{
	public class UnknownCommandHandler : BotCommandHandler
	{
		public string? Command => null;
		private readonly ITelegramBotClient _botClient;

		public UnknownCommandHandler(ITelegramBotClient botClient)
		{
			_botClient = botClient ?? throw new ArgumentNullException(nameof(botClient));
		}

		public async Task HandleAsync(Update update, CancellationToken cancellationToken)
		{
			await _botClient.SendTextMessageAsync(
				chatId: update.Message.Chat.Id,
				text: "Извините, но данная команда не распознана.",
				cancellationToken: cancellationToken);
		}

		public bool CanHandleCommand(Update update)
		{
			return true;
		}
	}
}
