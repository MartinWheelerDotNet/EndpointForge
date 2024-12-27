namespace EndpointManager.Abstractions.Models;

[Serializable]
public record AddEndpointRequest(
    string Route,
    string Method,
    EndpointRequestResponse? Response,
    int Priority = 0);

[Serializable]
public record EndpointRequestResponse(int StatusCode);