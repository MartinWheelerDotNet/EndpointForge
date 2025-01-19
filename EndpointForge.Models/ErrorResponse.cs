namespace EndpointForge.Models;

public record ErrorResponse(
    string ErrorStatusCode,
    string Message, 
    IEnumerable<string>? Errors);