using EndpointForge.Abstractions.Interfaces;
using EndpointForge.WebApi.Factories;
using EndpointForge.WebApi.Tests.Fakes;
using FluentAssertions;

namespace EndpointForge.WebApi.Tests.Factories;

public class EndpointForgeRuleFactoryTests
{
    [Fact]
    public void When_GetGeneratorRuleAndRuleIsNotFound_ThenReturnsNull()
    {
        var rules = new List<IEndpointForgeGeneratorRule>
        {
            new FakeGeneratorRule()
        };
        var ruleFactory = new EndpointForgeRuleFactory(rules);
        
        var rule = ruleFactory.GetGeneratorRule("generator:test");
        
        rule.Should().BeNull();
    }
    
    [Fact]
    public void When_GetGeneratorRuleAndRuleIsFound_ThenReturnsRule()
    {
        var generatorRule = new FakeGeneratorRule("generator:test");
        var rules = new List<IEndpointForgeGeneratorRule> { generatorRule };
        var ruleFactory = new EndpointForgeRuleFactory(rules);

        var rule = ruleFactory.GetGeneratorRule("generator:test");
        
        rule.Should().Be(generatorRule);
    }
}