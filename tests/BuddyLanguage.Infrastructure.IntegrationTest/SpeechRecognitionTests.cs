using BuddyLanguage.Domain.Exceptions;
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
                WhisperSpeechRecognitionService whisperService = new(null!);
            })
               .Should()
               .Throw<ArgumentNullException>();
        }

        [Fact]
        public async Task Voice_message_converts_to_text_successfully()
        {
            string fileName = @"assets/History.mp3";
            byte[] bytes = await File.ReadAllBytesAsync(fileName);

            var service = new OpenAIService(new OpenAiOptions()
            {
                ApiKey = GetKeyFromEnvironment("OPENAI_API_KEY"),
            });

            IOpenAIService openAIService = service;
            WhisperSpeechRecognitionService whisperService = new(openAIService);

            await FluentActions.Invoking(async () =>
            await whisperService.RecognizeSpeechToTextAsync(bytes, fileName, default))
                .Should().NotThrowAsync();
        }

        [Fact]
        public async Task Whisper_service_rejects_invalid_file_formats()
        {
            string fileName = @"assets/History.aiff";
            byte[] bytes = File.ReadAllBytes(fileName);

            var service = new OpenAIService(new OpenAiOptions()
            {
                ApiKey = GetKeyFromEnvironment("OPENAI_API_KEY"),
            });

            IOpenAIService openAIService = service;
            WhisperSpeechRecognitionService whisperService = new(openAIService);

            await FluentActions.Invoking(async () =>
            {
                await whisperService.RecognizeSpeechToTextAsync(bytes, fileName, default);
            }).Should().ThrowAsync<InvalidSpeechRecognitionException>();
        }

        [Theory]
        [InlineData(null, "VoiceMessage.mp3")]
        [InlineData(new byte[] { 1, 2, 3 }, null)]
        [InlineData(new byte[0], "VoiceMessage.mp3")]
        [InlineData(null, null)]
        public async Task Whisper_service_rejects_files_with_incorrect_data(
            byte[] bytes,
            string fileName)
        {
            var service = new OpenAIService(new OpenAiOptions()
            {
                ApiKey = GetKeyFromEnvironment("OPENAI_API_KEY"),
            });

            IOpenAIService openAIService = service;
            WhisperSpeechRecognitionService whisperService = new(openAIService);

            await FluentActions.Invoking(async () =>
            {
                await whisperService.RecognizeSpeechToTextAsync(bytes, fileName, default);
            }).Should().ThrowAsync<ArgumentException>();
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
