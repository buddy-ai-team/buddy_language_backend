using BuddyLanguage.ChatGPTServiceLib;
using BuddyLanguage.Domain.Enumerations;
using BuddyLanguage.Domain.Interfaces;
using BuddyLanguage.Domain.Services;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using OpenAI.ChatGpt;
using OpenAI.ChatGpt.AspNetCore;
using OpenAI.ChatGpt.Models;

namespace BuddyLanguage.Infrastructure.IntegrationTest;

public class BuddyServiceTests
{
    [Fact]
    public async Task Grammar_checking_missing_modal_verb__am__is_found()
    {
        // Arrange
        var openaiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");
        if (openaiKey == null)
        {
            throw new InvalidOperationException("OPENAI_API_KEY environment variable is not set");
        }

        var mockOptions = new Mock<IOptionsSnapshot<ChatGPTConfig>>();
        mockOptions.Setup(snapshot => snapshot.Value)
            .Returns(new ChatGPTConfig());

        var openaiClient = new OpenAiClient(openaiKey);
        var chatGptService = new ChatGPTService(
            ChatGPTFactory.CreateInMemory(openaiKey),
            openaiClient,
            mockOptions.Object);

        var buddyService = new BuddyService(
            chatGptService,
            Mock.Of<ISpeechRecognitionService>(),
            Mock.Of<ITextToSpeech>(),
            Mock.Of<IWordService>(),
            Mock.Of<ILogger<BuddyService>>());

        // Act
        var mistakes = await buddyService.FindGrammarMistakes(
            "I ready", Language.Russian, Language.English, CancellationToken.None);

        // Assert
        mistakes.Should().NotBeNull();
        mistakes.MistakesCount.Should().Be(1);
        mistakes.Mistakes.Should().NotBeNull();
        mistakes.Mistakes.Should().HaveCount(1);
        mistakes.Mistakes.Should().ContainMatch("*am*");
    }
}
