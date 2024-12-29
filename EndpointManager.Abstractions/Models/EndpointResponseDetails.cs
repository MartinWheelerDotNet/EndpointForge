namespace EndpointManager.Abstractions.Models;

[Serializable]
public record EndpointResponseDetails
{
    public int StatusCode { get; init; } = 200;
    public string? ContentType { get; init; } = null;
    public string? Body {get; init;} = null;
}