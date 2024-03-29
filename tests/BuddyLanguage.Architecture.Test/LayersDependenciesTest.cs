using System.Reflection;
using BuddyLanguage.Domain.Services;
using BuddyLanguage.Infrastructure;
using BuddyLanguage.TelegramBot;
using BuddyLanguage.TelegramBot.Services;
using BuddyLanguage.WebApi.Extensions;
using FluentAssertions;
using NetArchTest.Rules;

namespace BuddyLanguage.Architecture.Test;

public class LayersDependenciesTest
{
    /// <summary>
    /// Перечень существующих сборок
    /// </summary>
    private static readonly Assembly InfrastructureAssembly = typeof(BuddyLanguageDependencyInjection).Assembly;
    private static readonly Assembly PresentationWebApiAssembly = typeof(WebApiIntegrationExtensions).Assembly;
    private static readonly Assembly PresentationTelegramBotAssembly = typeof(TelegramUserService).Assembly;
    private static readonly Assembly DomainAssembly = typeof(RoleService).Assembly;

    /// <summary>
    /// Gets перечень существующих namespace
    /// </summary>
    private static string PresentationNamespace => PresentationTelegramBotAssembly.GetName().Name!;

    private static string DomainNamespace => DomainAssembly.GetName().Name!;

    private static string InfrastructureNamespace => InfrastructureAssembly.GetName().Name!;

    /// <summary>
    /// Определение всех Types доступных в проекте
    /// </summary>
    // ReSharper disable once InconsistentNaming
#pragma warning disable SA1201
    private static readonly Types Types =
#pragma warning restore SA1201
        Types.InAssemblies(new[]
        {
            InfrastructureAssembly,
            PresentationWebApiAssembly,
            PresentationTelegramBotAssembly,
            DomainAssembly
        });

    [Fact]
    public void Domain_not_depends_on_presentation()
    {
        Types.That()
            .ResideInNamespace(DomainNamespace)
            .ShouldNot().HaveDependencyOn(PresentationNamespace)
            .GetResult().IsSuccessful
            .Should().BeTrue();
    }

    [Fact]
    public void Domain_not_depends_on_infrastructure()
    {
        Types.That()
            .ResideInNamespace(DomainNamespace)
            .ShouldNot().HaveDependencyOn(InfrastructureNamespace)
            .GetResult().IsSuccessful
            .Should().BeTrue();
    }

    [Fact]
    public void Infrastructure_not_depends_on_presentation()
    {
        Types.That()
            .ResideInNamespace(InfrastructureNamespace)
            .ShouldNot().HaveDependencyOn(PresentationNamespace)
            .GetResult().IsSuccessful
            .Should().BeTrue();
    }
}
