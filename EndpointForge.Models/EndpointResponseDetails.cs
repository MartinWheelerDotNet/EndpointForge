namespace EndpointForge.Models;

public record EndpointResponseDetails
{
    public int StatusCode { get; set; } = 200;
    public string? ContentType { get; init; }
    public string? Body {get; set; }
}