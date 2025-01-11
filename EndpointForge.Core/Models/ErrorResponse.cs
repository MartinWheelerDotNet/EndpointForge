namespace EndpointForge.Core.Models;

public record ErrorResponse(HttpStatusCode StatusCode, string Message, IEnumerable<string>? Errors);