using System.Diagnostics.CodeAnalysis;
using EndpointForge.Abstractions.Interfaces;

namespace EndpointForge.WebApi.Tests.Fakes;

[ExcludeFromCodeCoverage]
internal class FakeEndpointForgeRuleFactory(string? content) : IEndpointForgeRuleFactory
{
    public IEndpointForgeGeneratorRule? GetGeneratorRule(ReadOnlySpan<char> placeholder)
        => content is null
            ? null
            : new FakeGeneratorRule(placeholder.ToString(), content);
    
}