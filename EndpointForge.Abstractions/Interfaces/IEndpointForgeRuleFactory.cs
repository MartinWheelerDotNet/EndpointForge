namespace EndpointForge.Abstractions.Interfaces;

public interface IEndpointForgeRuleFactory
{ 
    IEndpointForgeGeneratorRule? GetGeneratorRule(ReadOnlySpan<char> placeholder);
}