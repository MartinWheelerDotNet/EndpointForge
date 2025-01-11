using EndpointForge.Core.Abstractions;
using EndpointForge.WebApi.Factories;
using FluentAssertions;
using Moq;

namespace EndpointForge.WebApi.Tests.Factories;

public class EndpointForgeRuleFactoryTests
{
    private readonly Mock<IEndpointForgeGeneratorRule> _stubEndpointForgeGeneratorRule;    

    public EndpointForgeRuleFactoryTests()
    {
        _stubEndpointForgeGeneratorRule = new Mock<IEndpointForgeGeneratorRule>();
        _stubEndpointForgeGeneratorRule
            .SetupGet(rule => rule.Type)
            .Returns("test-rule");
    }
    
    [Fact]
    public void When_GetRuleGeneratorRuleAndRuleIsNotFound_ThenReturnsNull()
    {
        var rules = new List<IEndpointForgeRule> { _stubEndpointForgeGeneratorRule.Object };
        
        var ruleFactory = new EndpointForgeRuleFactory(rules);
        var rule = ruleFactory.GetRule<IEndpointForgeGeneratorRule>("unknown-rule");
        
        rule.Should().BeNull();
    }
    
    [Fact]
    public void When_GetRuleGeneratorRuleAndRuleIsFound_ThenReturnsRule()
    {
        var rules = new List<IEndpointForgeRule> { _stubEndpointForgeGeneratorRule.Object };
        
        var ruleFactory = new EndpointForgeRuleFactory(rules);
        var rule = ruleFactory.GetRule<IEndpointForgeGeneratorRule>("test-rule");
        
        rule.Should().Be(_stubEndpointForgeGeneratorRule.Object);
    }
}