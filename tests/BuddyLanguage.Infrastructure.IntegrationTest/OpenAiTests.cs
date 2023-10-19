using OpenAI.ChatGpt;
using OpenAI.ChatGpt.Models.ChatCompletion;

namespace BuddyLanguage.Infrastructure.IntegrationTest
{
    public class OpenAiTests
    {
        [Fact]
        public async void OpenAI_key_is_valid()
        {
            // Arrange
            var openaiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");
            if (openaiKey == null)
            {
                throw new InvalidOperationException("OPENAI_API_KEY environment variable is not set");
            }
            var openaiClient = new OpenAiClient(openaiKey);

            // Act
            var answer = await openaiClient.GetChatCompletions(
                Dialog.StartAsUser("Hello"),
                maxTokens: 3,
                model: ChatCompletionModels.Gpt3_5_Turbo
            );

            // Assert
            Assert.NotNull(answer);
        }
    }
}
