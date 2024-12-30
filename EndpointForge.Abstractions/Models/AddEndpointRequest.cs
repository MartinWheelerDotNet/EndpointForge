namespace EndpointForge.Abstractions.Models;

[Serializable]
public record AddEndpointRequest
{
    public required string Route { get; init; }
    public required List<string> Methods { get; init; } = [];
    public EndpointResponseDetails Response { get; init; } = new();
    public int Priority { get; init; }
    
    public IEnumerable<EndpointRoutingDetails> GetEndpointRoutingDetails() => 
        Methods.Select(method => new EndpointRoutingDetails(Route, method, Priority));
}