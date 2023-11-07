using BuddyLanguage.ChatGPTServiceLib;
using BuddyLanguage.Domain.Enumerations;
using BuddyLanguage.OpenAIWhisperSpeechRecognitionService;
using BuddyLanguage.TextToSpeech;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenAI;
using OpenAI.ChatGpt;
using OpenAI.ChatGpt.AspNetCore;
using OpenAI.ChatGpt.AspNetCore.Models;
using OpenAI.ChatGpt.Interfaces;
using OpenAI.ChatGpt.Internal;
using OpenAI.ChatGpt.Models;
using OpenAI.Interfaces;
using OpenAI.Managers;

namespace BuddyLanguage.Infrastructure.IntegrationTest
{


    public class PocTest
    {
        [Fact]
        public async Task PoC_user_story_works()
        {
            /* Arrange */
            var openAiOptions = new OpenAiOptions()
            {
                ApiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY") ?? throw new InvalidOperationException()
            };
            var service = new OpenAIService(openAiOptions);
            IOpenAIService openAIService = service;

            WhisperSpeechRecognitionService speechRecognitionService = new(openAIService);
            var chatGptConfig = new ChatGPTConfig();
            var chatGptFactory = new ChatGPTFactory(new HttpClientFactory(), Options.Create(new OpenAICredentials()), Options.Create(chatGptConfig), new ChatHistoryStorage(), new TimeProviderUtc());
            var chatGptService = new ChatGPTService(chatGptFactory, new OpenAiClient(openAiOptions.ApiKey), (IOptionsSnapshot<ChatGPTConfig>)Options.Create(chatGptConfig));

            var logger = new LoggerFactory().CreateLogger<AzureTextToSpeech>();
            var options = Options.Create(new AzureTTSConfig()
            {
                SpeechKey = GetKeyFromEnvironment("AZURE_SPEECH_KEY"),
                SpeechRegion = GetKeyFromEnvironment("AZURE_SPEECH_REGION")
            });

            var textToSpeechService = new AzureTextToSpeech(options, logger);
            var userId = Guid.NewGuid();
            string fileName = @"assets/History.mp3";
            byte[] bytes = await File.ReadAllBytesAsync(fileName);

            /* Act */
            var recognizedMessage = await speechRecognitionService.RecognizeSpeechToTextAsync(bytes, fileName, default);
            var chatbotResponse = await chatGptService.GetAnswerOnTopic(recognizedMessage, userId, default);
            var answerVoiceBytes = await textToSpeechService.TextToWavByteArrayAsync(chatbotResponse, Language.English, Voice.Male, default);

            /* Assert */

            // Проверка 1: Убедимся, что распознанный текст не пустой
            Assert.False(string.IsNullOrEmpty(recognizedMessage), "Распознанный текст не должен быть пустым.");

            // Проверка 2: Проверим, что бот возвращает ответ
            Assert.False(string.IsNullOrEmpty(chatbotResponse), "Ответ бота не должен быть пустым.");

            // Проверка 3: Проверим, что ответ бота содержит информацию об ошибках грамматики
            Assert.Contains("Ошибка грамматики:", chatbotResponse);

            // Проверка 4: Проверим, что ответ бота содержит продолжение диалога
            Assert.Contains("Продолжение диалога:", chatbotResponse);

            // Проверка 5: Убедимся, что озвученный голосовой ответ не пустой
            Assert.NotNull(answerVoiceBytes);

            // Проверка 6: Проверим, что бот запоминает контекст и использует его в диалоге
            var userMessage1 = "Hello";
            var chatbotResponseWithContext1 = await chatGptService.GetAnswerOnTopic(userMessage1, userId, default);
            Assert.Contains("Hello! How can i help you today?", chatbotResponseWithContext1);

            // Дополнительные проверки 

            // Проверка 7: Проверим, что бот обрабатывает ошибки и возвращает информацию об ошибках (если применимо)
            // Пример: var errorMessage = await chatGptService.GetAnswerFromChatGPT("This is invalid input", userId, default);
            // Assert.Contains("Invalid input:", errorMessage, "Бот должен сообщить об ошибке ввода.");

            // Проверка 8: Проверим, что бот корректно работает в разных языках и возвращает соответствующие ответы (если применимо)
            // Пример: var russianMessage = await chatGptService.GetAnswerFromChatGPT("Привет", userId, default);
            // Assert.Contains("Здравствуйте!", russianMessage, "Бот должен корректно работать на русском языке.");       
        }

        private string GetKeyFromEnvironment(string keyName)
        {
            if (keyName == null)
            {
                throw new ArgumentNullException(nameof(keyName));
            }

            var value = Environment.GetEnvironmentVariable(keyName);
            if (value is null)
            {
                throw new InvalidOperationException($"{keyName} is not set as environment variable");
            }

            return value;
        }

        private class HttpClientFactory : IHttpClientFactory
        {
            public HttpClient CreateClient(string name)
            {
                // Возвращаем новый экземпляр HttpClient
                return new HttpClient();
            }
        }

        private class ChatHistoryStorage : IChatHistoryStorage
        {
            public Task AddTopic(Topic topic, CancellationToken cancellationToken)
            {
                return Task.CompletedTask;
            }

            public Task<bool> ClearMessages(string userId, Guid topicId, CancellationToken cancellationToken)
            {
                return Task.FromResult(true);
            }

            public Task<bool> ClearTopics(string userId, CancellationToken cancellationToken)
            {
                return Task.FromResult(true);
            }

            public Task<bool> DeleteMessage(string userId, Guid topicId, Guid messageId, CancellationToken cancellationToken)
            {
                return Task.FromResult(true);
            }

            public Task<bool> DeleteTopic(string userId, Guid topicId, CancellationToken cancellationToken)
            {
                return Task.FromResult(true);
            }

            public Task EditMessage(string userId, Guid topicId, Guid messageId, string newMessage, CancellationToken cancellationToken)
            {
                return Task.CompletedTask;
            }

            public Task EditTopicName(string userId, Guid topicId, string newName, CancellationToken cancellationToken)
            {
                return Task.CompletedTask;
            }

            public Task EnsureStorageCreated(CancellationToken cancellationToken)
            {
                return Task.CompletedTask;
            }

            public Task<IEnumerable<PersistentChatMessage>> GetMessages(string userId, Guid topicId, CancellationToken cancellationToken)
            {
                return Task.FromResult(new List<PersistentChatMessage>().AsEnumerable());
            }

            public Task<Topic?> GetMostRecentTopicOrNull(string userId, CancellationToken cancellationToken)
            {
                return Task.FromResult((Topic?)null);
            }

            public Task<Topic> GetTopic(string userId, Guid topicId, CancellationToken cancellationToken)
            {
                throw new NotImplementedException();
            }

            public Task<IEnumerable<Topic>> GetTopics(string userId, CancellationToken cancellationToken)
            {
                return Task.FromResult(new List<Topic>().AsEnumerable());
            }

            public Task SaveMessages(string userId, Guid topicId, IEnumerable<PersistentChatMessage> messages, CancellationToken cancellationToken)
            {
                return Task.CompletedTask;
            }
        }
    }
}
