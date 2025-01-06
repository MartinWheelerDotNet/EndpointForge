using System.Collections.Immutable;

namespace EndpointForge.Abstractions.Models;

public record AddEndpointRequest
{
    public required string Route { get; init; } = string.Empty;
    public required List<string> Methods { get; init; } = [];
    public EndpointResponseDetails Response { get; init; } = new();
    public List<EndpointForgeParameterDetails> Parameters { get; init; } = [];
    
    public IEnumerable<EndpointRoutingDetails> GetEndpointRoutingDetails() => 
        Methods.Select(method => new EndpointRoutingDetails(Route, method));
}