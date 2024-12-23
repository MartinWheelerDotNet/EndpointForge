namespace EndpointManager.Abstractions.Models;

public record AddEndpointRequest(string Uri, HttpMethod HttpMethod);
