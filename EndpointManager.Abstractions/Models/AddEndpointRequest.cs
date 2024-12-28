namespace EndpointManager.Abstractions.Models;

[Serializable]
public record AddEndpointRequest
{
    public required string Route { get; init; }
    public required string[] Methods { get; init; } = [];
    public EndpointResponseDetails Response { get; init; } = new(200);
    public int Priority { get; init; } = 0;
    
    public IEnumerable<EndpointRoutingDetails> EndpointRoutingDetails => 
        Methods.Select(method => new EndpointRoutingDetails(Route, method, Priority));
}