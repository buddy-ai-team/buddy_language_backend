using OpenAI.ChatGpt.AspNetCore;
using OpenAI.ChatGpt;
using BuddyLanguage.Domain.Interfaces;

namespace BuddyLanguage.ChatGPTService
{
    public class ChatGPTService : IChatGPTService
    {
        private readonly ChatGPTFactory _chatGptFactory;

        public ChatGPTService(ChatGPTFactory chatGptFactory)
        {
            _chatGptFactory = chatGptFactory ?? throw new ArgumentNullException(nameof(chatGptFactory));
        }

        public async Task<string> GetAnswerFromChatGPT(string userMessage, Guid userId, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(userMessage))
            {
                throw new ArgumentException($"\"{nameof(userMessage)}\" it cannot be indefinite or empty.", nameof(userMessage));
            }

            ChatGPT chatGpt = await _chatGptFactory.Create(userId.ToString(), cancellationToken: cancellationToken);
            var chatService = await chatGpt.ContinueOrStartNewTopic(cancellationToken);
            var answer = await chatService.GetNextMessageResponse(userMessage, cancellationToken);

            return answer;
        }
    }
}