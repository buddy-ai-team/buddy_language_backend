using BuddyLanguage.Domain.Enumerations;
using BuddyLanguage.Domain.Interfaces;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuddyLanguage.Infrastructure.IntegrationTest
{
    /// <summary>
    /// Test class for Poc
    /// </summary>
    public class PocTest
    {
        [Fact]
        public async Task PoC_user_story_works()
        {
            // Arrange
            var voiceBytes = GetTgVoiceBytes();
            var userId = Guid.NewGuid();

            // Создаем моки для сервисов
            var mockSpeechRecognitionService = new Mock<ISpeechRecognitionService>();
            var mockChatGptService = new Mock<IChatGPTService>();
            var mockTextToSpeechService = new Mock<ITextToSpeech>();

            // Ожидаемые значения и настройка моков
            string recognizedMessage = "Hello";
            string chatbotResponse = "Hello! What is your name?";
            byte[] answerVoiceMessage = new byte[] { 1, 2, 3, 4 }; //пирмер байтов аудио

            mockSpeechRecognitionService.Setup(service => service.RecognizeSpeechToTextAsync(
         It.IsAny<byte[]>(), It.IsAny<string>(), default))
         .ReturnsAsync(recognizedMessage);

            mockChatGptService.Setup(service => service.GetAnswerFromChatGPT(
                recognizedMessage, userId, default))
                .ReturnsAsync(chatbotResponse);

            mockTextToSpeechService.Setup(service => service.TextToWavByteArrayAsync(
                chatbotResponse, Language.English, Voice.Male, default))
                .ReturnsAsync(answerVoiceMessage);

            // Act
            var speechRecognitionService = mockSpeechRecognitionService.Object;
            var chatGptService = mockChatGptService.Object;
            var textToSpeechService = mockTextToSpeechService.Object;

            recognizedMessage = await speechRecognitionService
                .RecognizeSpeechToTextAsync(voiceBytes, "testFileName", default);
            chatbotResponse = await chatGptService.GetAnswerFromChatGPT(recognizedMessage, userId, default);
            answerVoiceMessage = await textToSpeechService.TextToWavByteArrayAsync(chatbotResponse, Language.English, Voice.Male, default);

            // Assert
            recognizedMessage.Should().NotBeNullOrEmpty();
            chatbotResponse.Should().NotBeNullOrEmpty();
            answerVoiceMessage.Should().NotBeNullOrEmpty();
        }

        private byte[] GetTgVoiceBytes()
        {
            // Здесь возвращайте фактический байтовый массив звукового сообщения Telegram
            return new byte[] { 1, 2, 3 }; // Пример байтов аудио.
        }

    }
}
