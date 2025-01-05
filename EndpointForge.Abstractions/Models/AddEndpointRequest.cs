using System.Collections.Immutable;

namespace EndpointForge.Abstractions.Models;

[Serializable]
public record AddEndpointRequest
{
    public required string Route { get; init; } = string.Empty;
    public required ImmutableList<string> Methods { get; init; } = [];
    public EndpointResponseDetails Response { get; init; } = new();
    public ImmutableList<EndpointForgeParameterDetails> Parameters { get; init; } = [];
    
    public IEnumerable<EndpointRoutingDetails> GetEndpointRoutingDetails() => 
        Methods.Select(method => new EndpointRoutingDetails(Route, method));
}