using OpenAI.ChatGpt.AspNetCore;
using OpenAI.ChatGpt;
using BuddyLanguage.Domain.Interfaces;
using OpenAI.ChatGpt.Models.ChatCompletion.Messaging;
using System.Threading.Channels;
using System.Text;

namespace BuddyLanguage.ChatGPTServiceLib
{
    public class ChatGPTService : IChatGPTService
    {
        private readonly ChatGPTFactory _chatGptFactory;
        private readonly IOpenAiClient _openAiClient;

        public ChatGPTService(ChatGPTFactory chatGptFactory, IOpenAiClient openAiClient)
        {
            _chatGptFactory = chatGptFactory ?? throw new ArgumentNullException(nameof(chatGptFactory));
            _openAiClient = openAiClient ?? throw new ArgumentNullException(nameof(openAiClient));
        }

        public async Task<string> GetAnswerOnTopic(string userMessage, Guid userId, CancellationToken cancellationToken)
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

        public async Task<string> GetAnswer(string userMessage, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(userMessage))
            {
                throw new ArgumentException($"\"{nameof(userMessage)}\" it cannot be indefinite or empty.", nameof(userMessage));
            }

            var dialog = Dialog.StartAsUser(userMessage);

            var answer = await _openAiClient.GetChatCompletions(dialog, cancellationToken: cancellationToken);

            return answer;
        }

        public async Task<string> GetAnswer(string  prompt, string userMessage, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(prompt))
            {
                throw new ArgumentException($"\"{nameof(prompt)}\" it cannot be indefinite or empty.", nameof(prompt));
            }

            if (string.IsNullOrEmpty(userMessage))
            {
                throw new ArgumentException($"\"{nameof(userMessage)}\" it cannot be indefinite or empty.", nameof(userMessage));
            }

            var dialog = Dialog.StartAsSystem(prompt).ThenUser(userMessage);

            var answer = await _openAiClient.GetChatCompletions(dialog, cancellationToken: cancellationToken);

            return answer;
        }

    }
}