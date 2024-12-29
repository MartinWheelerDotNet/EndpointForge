namespace EndpointManager.Abstractions.Models;

[Serializable]
public record EndpointRoutingDetails(string Route, string Method, int Priority);