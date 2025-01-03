using System.Diagnostics.CodeAnalysis;
using EndpointForge.Abstractions.Interfaces;

namespace EndpointForge.WebApi.Tests.Fakes;

[ExcludeFromCodeCoverage]
internal class FakeEndpointForgeRuleFactory : IEndpointForgeRuleFactory
{
    private readonly Dictionary<ReadOnlyMemory<char>, IEndpointForgeRule> _rules;

    public FakeEndpointForgeRuleFactory(params IEndpointForgeRule[] fakeRules)
        => _rules = fakeRules.ToDictionary(rule => rule.Type.AsMemory(), rule => rule);

    public T? GetRule<T>(ReadOnlySpan<char> type) where T : IEndpointForgeRule
    {
        foreach (var (key, value) in _rules)
        {
            if (key.Span.SequenceEqual(type) && value is T rule)
                return rule;
        }

        return default;
    }
}