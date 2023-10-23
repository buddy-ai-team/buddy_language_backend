using FluentAssertions;
using BuddyLanguage.Domain.Exceptions;
using BuddyLanguage.OpenAIWhisperSpeechRecognitionService;
using OpenAI.Interfaces;
using OpenAI.Managers;
using OpenAI;
using BuddyLanguage.Domain.Interfaces;

namespace BuddyLanguage.Infrastructure.IntegrationTest
{
    public class SpeechRecognitionTests
    {
        [Fact]
        public void Whisper_service_creation_with_incorrect_data_is_rejected()
        {
            IOpenAIService openAIService = null;
            FluentActions.Invoking(() => 
            { 
                WhisperSpeechRecognitionService whisperService = new(openAIService); 
            })
               .Should()
               .Throw<ArgumentNullException>();
        }

        [Fact]
        public async Task Voicemessage_are_converted_to_text_succeeded()
        {
            string fileName = @"assets/History.mp3";
            byte[] bytes = File.ReadAllBytes(fileName);

            var service = new OpenAIService(new OpenAiOptions()
            {
                ApiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY"),
            });

            IOpenAIService openAIService = service;
            WhisperSpeechRecognitionService whisperService = new(openAIService);

            await FluentActions.Invoking(async () =>
            await whisperService.RecognizeSpeechToTextAsync(bytes, fileName, default))
                .Should().NotThrowAsync();
        }

        [Fact]
        public async Task Voicemessage_are_converted_to_text_with_unsupported_formats_rejected()
        {
            string fileName = @"assets/History.aiff";
            byte[] bytes = File.ReadAllBytes(fileName);

            var service = new OpenAIService(new OpenAiOptions()
            {
                ApiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY"),
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
        public async Task Voicemessage_are_converted_to_text_with_uncorrectly_data_rejected
            (byte[] bytes, string fileName)
        {
            var service = new OpenAIService(new OpenAiOptions()
            {
                ApiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY"),
            });

            IOpenAIService openAIService = service;
            WhisperSpeechRecognitionService whisperService = new(openAIService);

            await FluentActions.Invoking(async () =>
            {
                await whisperService.RecognizeSpeechToTextAsync(bytes, fileName, default);
            }).Should().ThrowAsync<ArgumentException>();
        }

        [Fact] 
        public void Whisperservice_implements_interface()
        {
            var service = new OpenAIService(new OpenAiOptions()
            {
                ApiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY"),
            });
            IOpenAIService openAIService = service;
            WhisperSpeechRecognitionService whisperService = new(openAIService);

            whisperService.Should().BeAssignableTo<ISpeechRecognitionService>();
        }
    }
}
