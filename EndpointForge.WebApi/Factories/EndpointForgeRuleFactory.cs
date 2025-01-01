using EndpointForge.Abstractions.Interfaces;

namespace EndpointForge.WebApi.Factories;

public class EndpointForgeRuleFactory : IEndpointForgeRuleFactory
{
    private readonly Dictionary<ReadOnlyMemory<char>, IEndpointForgeGeneratorRule> _rules;

    public EndpointForgeRuleFactory(IEnumerable<IEndpointForgeGeneratorRule> generatorRules)
    {
        _rules = generatorRules.ToDictionary(rule => rule.Placeholder.AsMemory(), rule => rule);
    }
    
    public IEndpointForgeGeneratorRule? GetGeneratorRule(ReadOnlySpan<char> placeholder)
    {
        foreach (var rule in _rules)
        {
            if (rule.Key.Span.SequenceEqual(placeholder)) 
                return rule.Value;
        }

        return null;
    }
}