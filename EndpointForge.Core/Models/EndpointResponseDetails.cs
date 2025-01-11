namespace EndpointForge.Core.Models;

public class EndpointResponseDetails
{
    public int StatusCode { get; set; } = 200;
    public string? ContentType { get; init; }
    public string? Body {get; set; }

    [ExcludeFromCodeCoverage]
    public override string ToString() => $"StatusCode: {StatusCode}, ContentType: {ContentType}, Body: {Body}";
}