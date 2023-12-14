using BuddyLanguage.Domain.Exceptions.User;
using BuddyLanguage.Domain.Interfaces;
using OpenAI.ChatGpt.Interfaces;

namespace BuddyLanguage.ExternalStatisticsServiceLib
{
    public class ExternalStatisticsService : IStatisticsService<StatisticsResponse>
    {
        private readonly IChatHistoryStorage _chatHistoryStorage;
        private readonly IUserRepository _userRepository;

        public ExternalStatisticsService(IChatHistoryStorage chatHistoryStorage, IUserRepository userRepository)
        {
            _chatHistoryStorage = chatHistoryStorage ?? throw new ArgumentNullException(nameof(chatHistoryStorage));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        }

        public async Task<StatisticsResponse> GetCountOfDaysAndMessages(string id, CancellationToken cancellationToken)
        {
            var user = await _userRepository.FindUserByTelegramId(id, cancellationToken);

            if (user == null)
            {
                throw new UserNotFoundException("Пользователь не найден");
            }

            var topics = await _chatHistoryStorage.GetTopics(user.Id.ToString(), cancellationToken);
            var totalMessages = 0;
            int numbersDaysCommunication = 0;

            if (topics.Count() > 1)
            {
                var firstTopic = topics.First();
                var lastTopic = topics.Last();

                var messagesLastTopic = await _chatHistoryStorage.GetMessages(user.Id.ToString(), lastTopic.Id, cancellationToken);
                var lastMessageTime = messagesLastTopic.Last().CreatedAt;

                var start1 = firstTopic.CreatedAt;
                var end1 = lastMessageTime;
                var duration1 = (end1 - start1).TotalDays;
                numbersDaysCommunication = (int)duration1;
            }
            else if (topics.Count() == 1)
            {
                var singleTopic = topics.First();
                var messagesSingleTopic = await _chatHistoryStorage.GetMessages(user.Id.ToString(), singleTopic.Id, cancellationToken);

                var start2 = singleTopic.CreatedAt;
                var end2 = messagesSingleTopic.Last().CreatedAt;
                var duration2 = (end2 - start2).TotalDays;
                numbersDaysCommunication = (int)duration2;
            }

            foreach (var topic in topics)
            {
                var messages = await _chatHistoryStorage.GetMessages(user.Id.ToString(), topic.Id, cancellationToken);
                totalMessages += messages.Count();
            }

            return new StatisticsResponse
            {
                TotalMessages = totalMessages,
                NumbersDaysCommunication = numbersDaysCommunication
            };
        }
    }
}
