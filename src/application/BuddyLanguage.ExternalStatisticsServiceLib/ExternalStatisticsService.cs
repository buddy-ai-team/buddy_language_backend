using BuddyLanguage.Domain.Exceptions.User;
using BuddyLanguage.Domain.Interfaces;
using OpenAI.ChatGpt.Interfaces;

namespace BuddyLanguage.ExternalStatisticsServiceLib
{
    public class ExternalStatisticsService : IStatisticsService
    {
        private readonly IChatHistoryStorage _chatHistoryStorage;
        private readonly IUserRepository _userRepository;

        public ExternalStatisticsService(IChatHistoryStorage chatHistoryStorage, IUserRepository userRepository)
        {
            _chatHistoryStorage = chatHistoryStorage ?? throw new ArgumentNullException(nameof(chatHistoryStorage));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        }

        public async Task<Statistics> GetCountOfDaysAndMessages(string id, CancellationToken cancellationToken)
        {
            var user = await _userRepository.FindUserByTelegramId(id, cancellationToken);

            if (user == null)
            {
                throw new UserNotFoundException("Пользователь не найден");
            }

            var topics = await _chatHistoryStorage.GetTopics(user.Id.ToString(), cancellationToken);
            var topicsSort = topics.OrderBy(topic => topic.CreatedAt);
            var totalMessages = 0;
            int numbersDaysCommunication = 0;

            var firstTopic = topicsSort.First();
            var lastTopic = topicsSort.Last();
            numbersDaysCommunication = (int)(lastTopic.CreatedAt - firstTopic.CreatedAt).TotalDays;

            foreach (var topic in topics)
            {
                var messages = await _chatHistoryStorage.GetMessages(user.Id.ToString(), topic.Id, cancellationToken);
                totalMessages += messages.Count();
            }

            return new Statistics
            {
                TotalMessages = totalMessages,
                NumbersDaysCommunication = numbersDaysCommunication
            };
        }
    }
}
