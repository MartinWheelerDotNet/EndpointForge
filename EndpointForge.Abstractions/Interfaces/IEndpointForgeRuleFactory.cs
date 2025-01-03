namespace EndpointForge.Abstractions.Interfaces;

public interface IEndpointForgeRuleFactory
{ 
    T? GetRule<T>(ReadOnlySpan<char> type) where T : IEndpointForgeRule;
}