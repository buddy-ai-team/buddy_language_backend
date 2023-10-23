using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InlineQueryResults;
using Telegram.Bot.Types.ReplyMarkups;

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
			{
				if (message.Voice != null)
				{
					await ReceiveVoiceMessage(_botClient, message, cancellationToken);
				}
				return;
			}

			var action = messageText.Split(' ')[0] switch
			{
				"/start" => StartCommand(_botClient, message, cancellationToken),
				"/menu" => Usage(_botClient, message, cancellationToken), // Отправляет сообщение с инструкциями по использованию бота
				"/voice" => SendVoiceMessage(_botClient, message, cancellationToken)
			} ;
			Message sentMessage = await action;
			_logger.LogInformation("The message was sent with id: {SentMessageId}", sentMessage.MessageId);
		}
		static async Task<Message> StartCommand(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
		{
			const string welcomeMessage = "Привет! Я ваш бот. Чем могу помочь вам сегодня?\n" +
										  "Вот список доступных команд:\n" +
										  "/menu - показать это меню";

			return await botClient.SendTextMessageAsync(
				chatId: message.Chat.Id,
				text: welcomeMessage,
				replyMarkup: new ReplyKeyboardRemove(),
				cancellationToken: cancellationToken);
		}
		static async Task<Message> Usage(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
		{
			const string usage = "Usage:\n" +
								 "/voice";

			return await botClient.SendTextMessageAsync(
				chatId: message.Chat.Id,
				text: usage,
				replyMarkup: new ReplyKeyboardRemove(),
				cancellationToken: cancellationToken);
		}

		// Обрабатываем голосовое сообщение пользователя
		static async Task<Message> ReceiveVoiceMessage(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
		{

			await botClient.SendChatActionAsync(
				message.Chat.Id,
				ChatAction.Typing,
				cancellationToken: cancellationToken);

			return await botClient.SendTextMessageAsync(
				chatId: message.Chat.Id,
				text: "Voice message received. Thank you!",
				cancellationToken: cancellationToken);
		}

		// Отправление голосового сообщения
		static async Task<Message> SendVoiceMessage(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
		{
			await botClient.SendChatActionAsync(
				message.Chat.Id,
				ChatAction.UploadVoice,
				cancellationToken: cancellationToken);

			const string filePath = "Files/voice_message.ogg"; // Путь к голосовому сообщению
			await using FileStream fileStream = new(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
			var fileName = filePath.Split(Path.DirectorySeparatorChar).Last();

			return await botClient.SendVoiceAsync(
				chatId: message.Chat.Id,
				voice: InputFile.FromStream(fileStream, fileName),
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
