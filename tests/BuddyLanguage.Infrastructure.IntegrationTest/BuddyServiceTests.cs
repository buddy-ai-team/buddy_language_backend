﻿using BuddyLanguage.ChatGPTServiceLib;
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

        var buddyService = new BuddyService(
            chatGptService,
            Mock.Of<ISpeechRecognitionService>(),
            Mock.Of<ITextToSpeech>(),
            Mock.Of<IWordService>(),
            Mock.Of<ILogger<BuddyService>>());

        // Act
        var mistakes = await buddyService.GetGrammarMistakesAndLearningWords(
            "I likes dog", Language.Russian, Language.English, CancellationToken.None);

        // Assert
        mistakes.Should().NotBeNull();
        mistakes.GrammaMistakesCount.Should().Be(1);
        mistakes.GrammaMistakes.Should().NotBeNull();
        mistakes.GrammaMistakes.Should().HaveCount(1);
        mistakes.GrammaMistakes.Should().ContainMatch("*like*");
        mistakes.WordsCount.Should().Be(0);
        mistakes.Words.Should().BeNullOrEmpty();
        mistakes.Words.Should().HaveCount(0);
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

        var buddyService = new BuddyService(
            chatGptService,
            Mock.Of<ISpeechRecognitionService>(),
            Mock.Of<ITextToSpeech>(),
            Mock.Of<IWordService>(),
            Mock.Of<ILogger<BuddyService>>());

        // Act
        var mistakes = await buddyService.GetGrammarMistakesAndLearningWords(
            "What is your любимый film?",
            Language.Russian,
            Language.English,
            CancellationToken.None);

        // Assert
        mistakes.Should().NotBeNull();
        mistakes.GrammaMistakesCount.Should().Be(0);
        mistakes.GrammaMistakes.Should().BeNullOrEmpty();
        mistakes.GrammaMistakes.Should().HaveCount(0);
        mistakes.WordsCount.Should().Be(1);
        mistakes.Words.Should().NotBeNull();
        mistakes.Words.Should().HaveCount(1);
        mistakes.Words.Should().ContainMatch("favorite");
    }
}
