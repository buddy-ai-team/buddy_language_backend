using NetArchTest.Rules;

namespace BuddyLanguage.Architecture.Test;

public class LayersDependenciesTest
{
    [Fact]
    public void domain_dependency_check()
    {
        var result1 =Types.InCurrentDomain()
            .That()
            .ResideInNamespace("BuddyLanguage.Domain")
            .ShouldNot()
            .HaveDependencyOn("BuddyLanguage.Data")
            .GetResult()
            .IsSuccessful;
        
        var result2 = Types.InCurrentDomain()
            .That()
            .ResideInNamespace("BuddyLanguage.Domain")
            .ShouldNot()
            .HaveDependencyOn("BuddyLanguage.WebApi")
            .GetResult()
            .IsSuccessful;
        
        var result3 = Types.InCurrentDomain()
            .That()
            .ResideInNamespace("BuddyLanguage.WebApi")
            .Should()
            .HaveDependencyOn("BuddyLanguage.Domain")
            .GetResult()
            .IsSuccessful;
    }
    
    [Fact]
    public void infrastructure_dependency_check()
    {
        var result1 = Types.InCurrentDomain()
            .That()
            .ResideInNamespace("BuddyLanguage.Data")
            .ShouldNot()
            .HaveDependencyOn("BuddyLanguage.WebApi")
            .GetResult()
            .IsSuccessful;
    }
}