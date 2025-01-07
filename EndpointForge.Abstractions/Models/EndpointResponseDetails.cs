namespace EndpointForge.Abstractions.Models;

public class EndpointResponseDetails
{
    public int StatusCode { get; set; } = 200;
    public string? ContentType { get; set; }
    public string? Body {get; set; }
}