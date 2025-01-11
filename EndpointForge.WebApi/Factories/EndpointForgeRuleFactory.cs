using EndpointForge.Abstractions;

namespace EndpointForge.WebApi.Factories;

public class EndpointForgeRuleFactory : IEndpointForgeRuleFactory
{
    private readonly Dictionary<ReadOnlyMemory<char>, IEndpointForgeRule> _rules;

    public EndpointForgeRuleFactory(IEnumerable<IEndpointForgeRule> endpointForgeRules)
    {
        var rulesList = endpointForgeRules.ToList();
        _rules = rulesList.ToDictionary(rule => rule.Type.AsMemory(), rule => rule);
    }
    
    public T? GetRule<T>(ReadOnlySpan<char> type) where T : IEndpointForgeRule
    {
        foreach (var (key, value) in _rules)
            if (key.Span.SequenceEqual(type) && value is T generatorRule) 
                return generatorRule;
        
        return default;
    }
}