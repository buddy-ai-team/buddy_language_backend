using System.Reflection;
using BuddyLanguage.Data.EntityFramework;
using BuddyLanguage.Domain.Services;
using BuddyLanguage.WebApi.Controllers;
using NetArchTest.Rules;
using FluentAssertions;

namespace BuddyLanguage.Architecture.Test;

public class LayersDependenciesTest
{
    
    /// <summary>
    /// Перечень существующих сборок
    /// </summary>
    private static readonly Assembly InfrastructureAssembly = typeof(AppDbContext).Assembly;
    private static readonly Assembly PresentationAssembly = typeof(RoleController).Assembly;
    private static readonly Assembly DomainAssembly = typeof(RoleService).Assembly;
    
    /// <summary>
    /// Перечень существующих namespace
    /// </summary>
    private static string PresentationNamespace => PresentationAssembly.GetName().Name!;
    private static string DomainNamespace => DomainAssembly.GetName().Name!;
    private static string InfrastructureNamespace => InfrastructureAssembly.GetName().Name!;

    /// <summary>
    /// Определение всех Types доступных в проекте
    /// </summary>
    private static readonly Types Types =
        Types.InAssemblies(new[] { 
            InfrastructureAssembly, 
            PresentationAssembly, 
            DomainAssembly 
        });
    
    [Fact]
    public void domain_not_depends_on_presentation()
    {
        Types.That()
            .ResideInNamespace(DomainNamespace)
            .ShouldNot().HaveDependencyOn(PresentationNamespace)
            .GetResult().IsSuccessful
            .Should().BeTrue();
    }
    
    [Fact]
    public void domain_not_depends_on_infrastructure()
    {
        Types.That()
            .ResideInNamespace(DomainNamespace)
            .ShouldNot().HaveDependencyOn(InfrastructureNamespace)
            .GetResult().IsSuccessful
            .Should().BeTrue();
    }
    
    [Fact]
    public void presentation_not_depends_on_infrastructure()
    {
        Types.That()
            .ResideInNamespace(PresentationNamespace)
            .ShouldNot().HaveDependencyOn(InfrastructureNamespace)
            .GetResult().IsSuccessful
            .Should().BeTrue();
    }
    
    [Fact]
    public void infrastructure_depends_on_domain()
    {
        Types.That()
            .ResideInNamespace(PresentationNamespace)
            .Should().HaveDependencyOn(DomainNamespace)
            .GetResult().IsSuccessful
            .Should().BeTrue();
    }
    
    [Fact]
    public void infrastructure_depends_on_presentation()
    {
        Types.That()
            .ResideInNamespace(PresentationNamespace)
            .Should().HaveDependencyOn(DomainNamespace)
            .GetResult().IsSuccessful
            .Should().BeTrue();
    }
}