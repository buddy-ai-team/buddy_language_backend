using BuddyLanguage.Domain.Interfaces;
using Microsoft.Extensions.Options;
using OpenAI.ChatGpt;
using OpenAI.ChatGpt.AspNetCore;
using OpenAI.ChatGpt.Models;
using OpenAI.ChatGpt.Models.ChatCompletion;
using OpenAI.ChatGpt.Modules.StructuredResponse;

namespace BuddyLanguage.ChatGPTServiceLib
{
    public class ChatGPTService : IChatGPTService
    {
        private readonly ChatGPTFactory _chatGptFactory;
        private readonly IOpenAiClient _openAiClient;
        private readonly ChatGPTConfig _config; // TODO ChatGPTModelsConfig
        private readonly string _model = ChatCompletionModels.Gpt3_5_Turbo;

        public ChatGPTService(
            ChatGPTFactory chatGptFactory,
            IOpenAiClient openAiClient,
            IOptionsSnapshot<ChatGPTConfig> chatgptOptions)
        {
            ArgumentNullException.ThrowIfNull(chatgptOptions);
            _chatGptFactory = chatGptFactory ?? throw new ArgumentNullException(nameof(chatGptFactory));
            _openAiClient = openAiClient ?? throw new ArgumentNullException(nameof(openAiClient));
            _config = chatgptOptions.Value ??
                      throw new ArgumentNullException($"{nameof(chatgptOptions)}.{nameof(chatgptOptions.Value)}");
        }

        public async Task<string> GetAnswerOnTopic(string userMessage, Guid userId, CancellationToken cancellationToken)
        {
            ArgumentException.ThrowIfNullOrEmpty(userMessage);
            _config.Model = _model; // TODO fix
            ChatGPT chatGpt = await _chatGptFactory.Create(
                userId.ToString(),
                _config,
                cancellationToken: cancellationToken);
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

        public async Task<string> GetAnswer(string prompt, string userMessage, CancellationToken cancellationToken)
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

            var answer = await _openAiClient.GetChatCompletions(
                dialog,
                model: _model,
                cancellationToken: cancellationToken);

            return answer;
        }

        public Task<TResult> GetStructuredAnswer<TResult>(
            string prompt, string userMessage, CancellationToken cancellationToken)
        {
            ArgumentException.ThrowIfNullOrEmpty(prompt);
            ArgumentException.ThrowIfNullOrEmpty(userMessage);

            var dialog = Dialog.StartAsSystem(prompt).ThenUser(userMessage);
            return _openAiClient.GetStructuredResponse<TResult>(
                dialog, model: _model, cancellationToken: cancellationToken);
        }

        public async Task ResetTopic(Guid userId, CancellationToken cancellationToken)
        {
            ChatGPT chatGpt = await _chatGptFactory.Create(userId.ToString(), cancellationToken: cancellationToken);
            await chatGpt.StartNewTopic(cancellationToken: cancellationToken);
        }
    }
}
