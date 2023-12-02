using BuddyLanguage.Domain.Entities;
using BuddyLanguage.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenAI.ChatGpt;
using OpenAI.ChatGpt.AspNetCore;
using OpenAI.ChatGpt.Models;
using OpenAI.ChatGpt.Models.ChatCompletion;
using OpenAI.ChatGpt.Models.ChatCompletion.Messaging;
using OpenAI.ChatGpt.Modules.StructuredResponse;
using Tiktoken;

namespace BuddyLanguage.ChatGPTServiceLib
{
    public class ChatGPTService : IChatGPTService
    {
        private readonly ChatGPTFactory _chatGptFactory;
        private readonly IOpenAiClient _openAiClient;
        private readonly ILogger<ChatGPTService> _logger;
        private readonly ChatGPTConfig _config; // TODO ChatGPTModelsConfig
        private readonly string _model = ChatCompletionModels.Gpt4Turbo;
        private readonly Encoding _dialogEncoding;

        public ChatGPTService(
            ChatGPTFactory chatGptFactory,
            IOpenAiClient openAiClient,
            IOptionsSnapshot<ChatGPTConfig> chatgptOptions,
            ILogger<ChatGPTService> logger)
        {
            ArgumentNullException.ThrowIfNull(chatgptOptions);
            _chatGptFactory = chatGptFactory ?? throw new ArgumentNullException(nameof(chatGptFactory));
            _openAiClient = openAiClient ?? throw new ArgumentNullException(nameof(openAiClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _config = chatgptOptions.Value ??
                      throw new ArgumentNullException($"{nameof(chatgptOptions)}.{nameof(chatgptOptions.Value)}");
            _dialogEncoding = Tiktoken.Encoding.Get(Encodings.Cl100KBase);
        }

        public async Task<string> GetAnswerOnTopic(
            string userMessage, Guid userId, Role role, CancellationToken cancellationToken)
        {
            ArgumentException.ThrowIfNullOrEmpty(userMessage);
            ArgumentNullException.ThrowIfNull(role);
            ChatService chatService = await GetChatServiceDialog(userId, role, cancellationToken);
            string answer = await chatService.GetNextMessageResponse(userMessage, cancellationToken);
            return answer;
        }

        public async Task<string> GetAnswer(string userMessage, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(userMessage))
            {
                throw new ArgumentException($"\"{nameof(userMessage)}\" it cannot be indefinite or empty.", nameof(userMessage));
            }

            var dialog = Dialog.StartAsUser(userMessage);
            var answer = await _openAiClient.GetChatCompletions(dialog, model: _model, cancellationToken: cancellationToken);
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

        private async Task<ChatService> GetChatServiceDialog(
            Guid userId,
            Role role,
            CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(role); 
            _config.Model = _model; // TODO fix
            ChatGPT chatGpt = await _chatGptFactory.Create(
                userId.ToString(),
                _config,
                cancellationToken: cancellationToken);
            var chatService = await chatGpt.ContinueOrStartNewTopic(cancellationToken);
            ChatCompletionMessage[] messages = (await chatService.GetMessages(cancellationToken))
                .Cast<ChatCompletionMessage>()
                .ToArray();
            if (messages.Length > 0)
            {
                var dialogText = string.Join('\n', messages.Select(it => it.Content));
                var tokenCount = _dialogEncoding.CountTokens(dialogText);
                if ((tokenCount + 500) > ChatCompletionModels.GetMaxTokensLimitForModel(_model))
                {
                    // TODO: делать суммаризацию после выполнения основной работы в фоне
                    _logger.LogInformation("Dialog is too long ({TokenCount}), compacting...", tokenCount);
                    var summaryPrompt = "Summarize the conversation so far, leave only important information and info about the user itself.";
                    messages[0] = new SystemMessage(summaryPrompt);
                    string summary = await _openAiClient.GetChatCompletions(
                        messages,
                        model: _model,
                        cancellationToken: cancellationToken);

                    string initialMessage =
                        $"{_config.InitialSystemMessage!}" +
                        $"\n\nRole: {role.Name}" +
                        $"\n\nPrompt: {role.Prompt}" +
                        $"\n\nContext: {summary}"; 
                    var dialog = Dialog.StartAsSystem(initialMessage);
                    chatService = await chatGpt.StartNewTopic(
                        "Summarized",
                        initialDialog: dialog,
                        cancellationToken: cancellationToken);
                }
            }

            return chatService;
        }
    }
}
