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
    public async Task Grammar_checking_irregular_form_of_the_verb__like__is_found()
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
            mockOptions.Object,
            Mock.Of<ILogger<ChatGPTService>>());

        var wordService = new WordService(
            Mock.Of<IUnitOfWork>(), Mock.Of<ILogger<WordService>>());

        var buddyService = new BuddyService(
            chatGptService,
            Mock.Of<ISpeechRecognitionService>(),
            Mock.Of<ITextToSpeech>(),
            Mock.Of<IPronunciationAssessmentService>(),
            wordService,
            Mock.Of<ILogger<BuddyService>>());

        // Act
        var mistakes = await buddyService.GetGrammarMistakes(
            "I likes dog", Language.Russian, CancellationToken.None);

        // Assert
        mistakes.Should().NotBeNull();
        mistakes.GrammaMistakesCount.Should().Be(1);
        mistakes.GrammaMistakes.Should().NotBeNull();
        mistakes.GrammaMistakes.Should().HaveCount(1);
        mistakes.GrammaMistakes.Should().ContainMatch("*like*");
    }

    [Fact]
    public async Task Checking_learning_word__favorite__is_found()
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
            mockOptions.Object,
            Mock.Of<ILogger<ChatGPTService>>());

        var wordService = new WordService(
            Mock.Of<IUnitOfWork>(), Mock.Of<ILogger<WordService>>());

        var buddyService = new BuddyService(
            chatGptService,
            Mock.Of<ISpeechRecognitionService>(),
            Mock.Of<ITextToSpeech>(),
            Mock.Of<IPronunciationAssessmentService>(),
            wordService,
            Mock.Of<ILogger<BuddyService>>());

        // Act
        var words = await buddyService.GetLearningWords(
            "What is your любимый film?",
            Language.Russian,
            Language.English,
            CancellationToken.None);

        // Assert
        words.Should().NotBeNull();
        words.WordsCount.Should().Be(1);
        words.StudiedWords.Should().NotBeNull();
        words.StudiedWords.Should().HaveCount(1);
        words.StudiedWords.ContainsKey("любимый");
        words.StudiedWords.ContainsValue("favorite");
    }
}
