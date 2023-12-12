using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BuddyLanguage.Domain.Entities;
using BuddyLanguage.Domain.Interfaces;
using OpenAI.ChatGpt.Interfaces;
using OpenAI.ChatGpt.Models;

namespace BuddyLanguage.ChatGPTServiceLib
{
    public class MessageRepository : IMessageRepository
    {
        private readonly IChatHistoryStorage _chatHistoryStorage;
        private readonly IUserRepository _userRepository;

        public MessageRepository(IChatHistoryStorage chatHistoryStorage, IUserRepository userRepository)
        {
            _chatHistoryStorage = chatHistoryStorage ?? throw new ArgumentNullException(nameof(chatHistoryStorage));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        }

        public async Task<int> GetMessageCount(string id, Guid topicId, CancellationToken cancellationToken)
        {
            var user = await _userRepository.FindUserByTelegramId(id, cancellationToken);
            var totalMessages = 0;
            if (user == null)
            {
                return 0;
            }
            else
            {
                var topics = await _chatHistoryStorage.GetTopics(user.Id.ToString(), cancellationToken);
                foreach (var topic in topics)
                {
                    var messages = await _chatHistoryStorage.GetMessages(user.Id.ToString(), topic.Id, cancellationToken);
                    var totalMessagesTopic = messages.Count();
                    totalMessages += totalMessagesTopic;
                }

                return totalMessages;
            }
        }

        public async Task<int> GetNumbersDaysCommunication(string id, CancellationToken cancellationToken)
        {
            var user = await _userRepository.FindUserByTelegramId(id, cancellationToken);
            if (user != null)
            {
                var topics = await _chatHistoryStorage.GetTopics(user.Id.ToString()!, cancellationToken);
                if (topics.Count() > 1)
                {
                    var firstTopic = topics.First();
                    var lastTopic = topics.Last();

                    var messagesLastTopic = await _chatHistoryStorage.GetMessages(user.Id.ToString(), lastTopic.Id, cancellationToken);
                    var lastMessageTime = messagesLastTopic.Last().CreatedAt;

                    var start1 = firstTopic.CreatedAt;
                    var end1 = lastMessageTime;
                    var duration1 = (end1 - start1).TotalDays;
                    return (int)duration1;
                }
                else if (topics.Count() == 1)
                {
                    var singlTopic = topics.First();
                    var messagesSingleTopic = await _chatHistoryStorage.GetMessages(user.Id.ToString(), singlTopic.Id, cancellationToken);

                    var start2 = singlTopic.CreatedAt;
                    var end2 = messagesSingleTopic.Last().CreatedAt;
                    var duration2 = (end2 - start2).TotalDays;
                    return (int)duration2;
                }
                else
                {
                    return 0;
                }
            }
            else
            {
                throw new Exception("Пользователь не найден");
            }
        }
    }
}
