using FluentAssertions;
using OpenAI.ChatGpt;
using OpenAI.ChatGpt.Models.ChatCompletion;
using OpenAI.ChatGpt.Modules.Translator;

namespace BuddyLanguage.Infrastructure.IntegrationTest
{
    public class OpenAiTests
    {
        [Fact]
        public async Task OpenAI_key_is_valid()
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
                model: ChatCompletionModels.Gpt3_5_Turbo);

            // Assert
            answer.Should().NotBeNull();
        }

        [Fact]
        public async Task Translation_from_russian_into_english_return_translation__processing__is_sucedeed()
        {
            //Arrange
            var text = "Обработка...";
            var sourceLanguage = "russian";
            var preferedLanguage = "english";

            var openaiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");
            if (openaiKey == null)
            {
                throw new InvalidOperationException("OPENAI_API_KEY environment variable is not set");
            }

            var openaiClient = new OpenAiClient(openaiKey);
            
            //Act
            var result = await openaiClient.TranslateText(
               text, sourceLanguage, preferedLanguage);

            //Assert
            result.Should().NotBeNull();
            result.Should().Contain("Processing");
        }
    }
}
