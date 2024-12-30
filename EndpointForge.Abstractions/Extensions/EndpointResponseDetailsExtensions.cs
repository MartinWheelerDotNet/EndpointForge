using EndpointForge.Abstractions.Models;

namespace EndpointForge.Abstractions.Extensions;

public static class EndpointResponseDetailsExtensions
{
    public static bool HasBody(this EndpointResponseDetails details) 
        => !string.IsNullOrWhiteSpace(details.Body);

    public static bool HasContentType(this EndpointResponseDetails details) 
        => !string.IsNullOrWhiteSpace(details.ContentType);
}