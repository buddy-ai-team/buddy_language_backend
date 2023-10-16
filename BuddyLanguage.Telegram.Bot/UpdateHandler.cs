using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;

namespace BuddyLanguage.Telegram.Bot
{
	public class UpdateHandler : IUpdateHandler
	{
		private readonly ITelegramBotClient _botClient;
		private readonly ILogger<UpdateHandler> _logger;

		public UpdateHandler(ITelegramBotClient botClient, ILogger<UpdateHandler> logger)
		{
			_botClient = botClient;
			_logger = logger;
		}

		public async Task HandleUpdateAsync(ITelegramBotClient tgBotClient, Update update, CancellationToken cancellationToken)
		{
			var handler = update switch
			{
				{ Message: { } message } => BotOnMessageReceived(message, cancellationToken),
				{ EditedMessage: { } message } => BotOnMessageReceived(message, cancellationToken),
				{ CallbackQuery: { } callbackQuery } => BotOnCallbackQueryReceived(callbackQuery, cancellationToken),
				{ InlineQuery: { } inlineQuery } => BotOnInlineQueryReceived(inlineQuery, cancellationToken),
				{ ChosenInlineResult: { } chosenInlineResult } => BotOnChosenInlineResultReceived(chosenInlineResult, cancellationToken),
				tgBotClient => UnknownUpdateHandlerAsync(update, cancellationToken)
			};
			await handler;
		}

	}
}
