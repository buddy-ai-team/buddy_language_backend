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

        public async Task<int> GetNumbersDaysCommunication(string userId, CancellationToken cancellationToken)
        {
            var topics = await _chatHistoryStorage.GetTopics(userId, cancellationToken);
            var totalDays = 0;

            /*foreach (var topic in topics)
            {
                var topicMessages = await _chatHistoryStorage.GetMessages(userId, topic.Id, cancellationToken);
                var firstMessageDate = topicMessages.Min(m => m.Timestamp).Date;
                var lastMessageDate = topicMessages.Max(m => m.Timestamp).Date;

                totalDays += (int)(lastMessageDate - firstMessageDate).TotalDays + 1; // Plus 1 to include both first and last days
            }*/

            return totalDays;
        }
    }
}
