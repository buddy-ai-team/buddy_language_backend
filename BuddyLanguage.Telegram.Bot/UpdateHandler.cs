using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InlineQueryResults;
using Telegram.Bot.Types.ReplyMarkups;
using File = System.IO.File;

namespace BuddyLanguage.Telegram.Bot
{
	public class UpdateHandler
	{
		// выполняет обработку различных событий и запросов от пользователей в Telegram боте, предоставляя разнообразные функциональные возможности
		private readonly ITelegramBotClient _botClient;
		private readonly ILogger<UpdateHandler> _logger;

		public UpdateHandler(ITelegramBotClient botClient, ILogger<UpdateHandler> logger)
		{
			_botClient = botClient;
			_logger = logger;
		}

		public async Task HandleUpdateAsync(ITelegramBotClient _, Update update, CancellationToken cancellationToken)
		{
			// функциональность клиента с ботом
			var handler = update switch
			{
				{ Message: { } message } => BotIsResponse(message, cancellationToken),
				{ EditedMessage: { } message } => BotIsResponse(message, cancellationToken),
				{ InlineQuery: { } inlineQuery } => BotOnInlineQueryReceived(inlineQuery, cancellationToken), // Формирует результаты встроенного запроса и отправляет их пользователю
				{ ChosenInlineResult: { } chosenInlineResult } => BotOnChosenInlineResultReceived(chosenInlineResult, cancellationToken), // Отправляет сообщение, оповещая пользователя о выборе определенного результата
				_ => UnknownUpdateHandlerAsync(update, cancellationToken) // Записывает информацию о неизвестном типе обновления в логи
			};
			await handler;
		}

		private async Task BotIsResponse(Message message, CancellationToken cancellationToken)
		{
			_logger.LogInformation("Receive message type: {MessageType}", message.Type);
			if (message.Text is not { } messageText)
				return;

			var action = messageText.Split(' ')[0] switch
			{
				"/photo" => SendFile(_botClient, message, cancellationToken), // Отправляет пользователю изображение (фотографию) в сообщении
				"/inline_mode" => StartInlineQuery(_botClient, message, cancellationToken), // Пользователь может нажать кнопку для начала взаимодействия с ботом

				"/menu" => Usage(_botClient, message, cancellationToken) // Отправляет сообщение с инструкциями по использованию бота
			};
			Message sentMessage = await action;
			_logger.LogInformation("The message was sent with id: {SentMessageId}", sentMessage.MessageId);
		}

		static async Task<Message> Usage(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
		{
			const string usage = "Usage:\n" +
								 "/photo       - send a photo\n" +
								 "/inline_mode - send keyboard with Inline Query";

			return await botClient.SendTextMessageAsync(
				chatId: message.Chat.Id,
				text: usage,
				replyMarkup: new ReplyKeyboardRemove(),
				cancellationToken: cancellationToken);
		}

		// Отправляет пользователю изображение(фотографию) в сообщении
		static async Task<Message> SendFile(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
		{
			await botClient.SendChatActionAsync(
				message.Chat.Id,
				ChatAction.UploadPhoto,
				cancellationToken: cancellationToken);

			const string filePath = "Files/tux.png";
			await using FileStream fileStream = new(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
			var fileName = filePath.Split(Path.DirectorySeparatorChar).Last();

			return await botClient.SendPhotoAsync(
				chatId: message.Chat.Id,
				photo: new InputFileStream(fileStream, fileName),
				caption: "Nice Picture",
				cancellationToken: cancellationToken);
		}
		// Пользователь может нажать кнопку для начала взаимодействия с ботом
		static async Task<Message> StartInlineQuery(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
		{
			InlineKeyboardMarkup inlineKeyboard = new(
				InlineKeyboardButton.WithSwitchInlineQueryCurrentChat("Inline Mode"));

			return await botClient.SendTextMessageAsync(
				chatId: message.Chat.Id,
				text: "Press the button to start Inline Query",
				replyMarkup: inlineKeyboard,
				cancellationToken: cancellationToken);
		}

		static async Task<Message> SendVoice(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
		{
			await botClient.SendChatActionAsync(
				message.Chat.Id,
				ChatAction.RecordVoice,
				cancellationToken: cancellationToken);

			const string filePath = "Files/voice_message.ogg";
			await using FileStream fileStream = new(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
			var fileName = filePath.Split(Path.DirectorySeparatorChar).Last();

			return await botClient.SendVoiceAsync(
				chatId: message.Chat.Id,
				voice: new InputOnlineFile(fileStream, fileName),
				caption: "Nice Voice Message",
				cancellationToken: cancellationToken);
		}

		// Формирует результаты встроенного запроса и отправляет их пользователю
		private async Task BotOnInlineQueryReceived(InlineQuery inlineQuery, CancellationToken cancellationToken)
		{
			_logger.LogInformation("Received inline query from: {InlineQueryFromId}", inlineQuery.From.Id);

			InlineQueryResult[] results = {
				new InlineQueryResultArticle(
					id: "1",
					title: "TgBots",
					inputMessageContent: new InputTextMessageContent("hello"))
			};

			await _botClient.AnswerInlineQueryAsync(
				inlineQueryId: inlineQuery.Id,
				results: results,
				cacheTime: 0,
				isPersonal: true,
				cancellationToken: cancellationToken);
		}

		// Отправляет сообщение, оповещая пользователя о выборе определенного результата
		private async Task BotOnChosenInlineResultReceived(ChosenInlineResult chosenInlineResult, CancellationToken cancellationToken)
		{
			_logger.LogInformation("Received inline result: {ChosenInlineResultId}", chosenInlineResult.ResultId);

			await _botClient.SendTextMessageAsync(
				chatId: chosenInlineResult.From.Id,
				text: $"You chose result with Id: {chosenInlineResult.ResultId}",
				cancellationToken: cancellationToken);
		}

		// Записывает информацию о неизвестном типе обновления в логи
		private Task UnknownUpdateHandlerAsync(Update update, CancellationToken cancellationToken)
		{
			_logger.LogInformation("Unknown update type: {UpdateType}", update.Type);
			return Task.CompletedTask;
		}

		// Определяет тип ошибки и записывает соответствующее сообщение об ошибке в логи
		public async Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
		{
			var ErrorMessage = exception switch
			{
				ApiRequestException apiRequestException => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
				_ => exception.ToString()
			};

			_logger.LogInformation("HandleError: {ErrorMessage}", ErrorMessage);

			if (exception is RequestException)
				await Task.Delay(TimeSpan.FromSeconds(2), cancellationToken);
		}
	}
}
