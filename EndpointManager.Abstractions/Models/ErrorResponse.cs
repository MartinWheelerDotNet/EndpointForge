namespace EndpointManager.Abstractions.Models;

public record ErrorResponse(IEnumerable<string> Errors);