using BuddyLanguage.Domain.Enumerations;
using BuddyLanguage.Domain.Interfaces;
using BuddyLanguage.OpenAIWhisperSpeechRecognitionService;
using FluentAssertions;
using OpenAI;
using OpenAI.Interfaces;
using OpenAI.Managers;

namespace BuddyLanguage.Infrastructure.IntegrationTest
{
    public class SpeechRecognitionTests
    {
        [Fact]
        public void Whisper_service_creation_with_incorrect_data_is_rejected()
        {
            FluentActions.Invoking(() =>
            {
                _ = new WhisperSpeechRecognitionService(null!);
            })
               .Should()
               .Throw<ArgumentNullException>();
        }

        [Fact]
        public async Task Voice_message_converts_to_text_successfully()
        {
            string fileName = "assets/History.mp3";
            byte[] bytes = await File.ReadAllBytesAsync(fileName);

            var service = new OpenAIService(new OpenAiOptions()
            {
                ApiKey = GetKeyFromEnvironment("OPENAI_API_KEY"),
            });

            IOpenAIService openAIService = service;
            WhisperSpeechRecognitionService whisperService = new(openAIService);

            await FluentActions.Invoking(async () =>
                await whisperService.RecognizeSpeechToTextAsync(bytes, AudioFormat.Mp3, Language.Russian, Language.English, default))
                .Should().NotThrowAsync();
        }

        [Fact]
        public void Whisper_service_implements_interface()
        {
            var service = new OpenAIService(new OpenAiOptions()
            {
                ApiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY") ?? throw new InvalidOperationException()
            });
            IOpenAIService openAIService = service;
            WhisperSpeechRecognitionService whisperService = new(openAIService);

            whisperService.Should().BeAssignableTo<ISpeechRecognitionService>();
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
    }
}
