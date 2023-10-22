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

        public async Task<string> GetAnswerWithoutContextOfDialog(string userMessage, CancellationToken cancellationToken, string? systemMessage = null)
        {
            if (string.IsNullOrEmpty(userMessage))
            {
                throw new ArgumentException($"\"{nameof(userMessage)}\" it cannot be indefinite or empty.", nameof(userMessage));
            }

            var dialog = ConfigDialog(userMessage, systemMessage);

            var answer = await _openAiClient.GetChatCompletions(dialog, cancellationToken: cancellationToken);

            return answer;
        }

        private UserOrSystemMessage ConfigDialog(string userMessage, string? systemMessage)
        {
            UserOrSystemMessage dialog;

            if (systemMessage is not null)
            {
                dialog = Dialog.StartAsSystem(systemMessage).ThenUser(userMessage);
            }
            else
            {
                dialog = Dialog.StartAsUser(userMessage);
            }

            return dialog;
        }
    }
}