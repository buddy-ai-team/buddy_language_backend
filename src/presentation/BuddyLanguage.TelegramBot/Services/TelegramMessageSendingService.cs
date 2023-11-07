using BuddyLanguage.Domain.Interfaces;
using OpenAI.ChatGpt.Interfaces;
using Telegram.Bot;

namespace BuddyLanguage.TelegramBot.Services
{
    public class TelegramMessageSendingService : IMessageSendingService
    {
        private readonly IChatGPTService _chatGPTSevice;
        private readonly ITelegramBotClient _botClient;
        private readonly IChatHistoryStorage _chatHistoryStorage;
        private readonly IUserRepository _userRepository;
        private readonly string _prompt = "The user does not appear in the application for a long time, you need to inspire him " +
                                          "to return, continue communication and further study the language. " +
                                          "Do it in a fun comic way.";

        public TelegramMessageSendingService(
                                             IChatGPTService chatGPTService,
                                             ITelegramBotClient botClient,
                                             IChatHistoryStorage chatHistoryStorage,
                                             IUserRepository userRepository)
        {
            _chatGPTSevice = chatGPTService ?? throw new ArgumentNullException(nameof(chatGPTService));
            _botClient = botClient ?? throw new ArgumentNullException(nameof(botClient));
            _chatHistoryStorage = chatHistoryStorage ?? throw new ArgumentNullException(nameof(chatHistoryStorage));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        }

        public async Task CheckAndSendReminder(int reminderIntervalHours, CancellationToken cancellationToken)
        {
            var users = await _userRepository.GetAll(cancellationToken);
            foreach (var user in users)
            {
                var lastTopic = await _chatHistoryStorage.GetMostRecentTopicOrNull(user.Id.ToString(), cancellationToken);
                if (lastTopic == null)
                {
                    return;
                }

                var messages = await _chatHistoryStorage.GetMessages(user.Id.ToString(), lastTopic.Id, cancellationToken);
                var lastMessageTime = messages.Last().CreatedAt;
                var currentTime = DateTime.Now;
                var afterLastMessageIntervalHours = (currentTime - lastMessageTime).TotalHours;

                if (afterLastMessageIntervalHours >= reminderIntervalHours)
                {
                    var reminder = await _chatGPTSevice.GetAnswer(_prompt, cancellationToken);
                    await _botClient.SendTextMessageAsync(
                        chatId: user.Id.ToString(),
                        text: reminder);
                }
            }
        }
    }
}
