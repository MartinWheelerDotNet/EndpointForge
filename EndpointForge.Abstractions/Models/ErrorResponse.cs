using System.Net;

namespace EndpointForge.Abstractions.Models;

public record ErrorResponse(HttpStatusCode StatusCode, string Message, IEnumerable<string>? Errors);

