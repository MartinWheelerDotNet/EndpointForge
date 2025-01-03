using EndpointForge.Abstractions.Interfaces;
using EndpointForge.WebApi.Factories;
using EndpointForge.WebApi.Tests.Fakes;
using FluentAssertions;

namespace EndpointForge.WebApi.Tests.Factories;

public class EndpointForgeRuleFactoryTests
{
    [Fact]
    public void When_GetRuleGeneratorRuleAndRuleIsNotFound_ThenReturnsNull()
    {
        var rules = new List<IEndpointForgeRule> { new FakeGeneratorRule() };
        var ruleFactory = new EndpointForgeRuleFactory(rules);
        
        var rule = ruleFactory.GetRule<IEndpointForgeGeneratorRule>("test");
        
        rule.Should().BeNull();
    }
    
    [Fact]
    public void When_GetRuleGeneratorRuleAndRuleIsFound_ThenReturnsRule()
    {
        var generatorRule = new FakeGeneratorRule("test");
        var rules = new List<IEndpointForgeRule> { generatorRule };
        var ruleFactory = new EndpointForgeRuleFactory(rules);

        var rule = ruleFactory.GetRule<IEndpointForgeGeneratorRule>("test");
        
        rule.Should().Be(generatorRule);
    }
}