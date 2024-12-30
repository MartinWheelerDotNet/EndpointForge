namespace EndpointForge.Abstractions.Models;

[Serializable]
public record EndpointResponseDetails
{
    public int StatusCode { get; init; } = 200;
    public string? ContentType { get; init; }
    public string? Body {get; init;}
}