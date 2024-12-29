using EndpointManager.Abstractions.Models;

namespace EndpointManager.Abstractions.Extensions;

public static class EndpointResponseDetailsExtensions
{
    public static bool HasBody(this EndpointResponseDetails details) 
        => !string.IsNullOrWhiteSpace(details.Body);

    public static bool HasContentType(this EndpointResponseDetails details) 
        => !string.IsNullOrWhiteSpace(details.ContentType);
}