namespace EndpointManager.Abstractions.Models;

[Serializable]
public record AddEndpointRequest(string Route, string Method, int Priority = 0);