namespace EndpointManager.Abstractions.Models;

public record ErrorResponse(string? Uri, HttpMethod? HttpMethod, string ErrorMessage);
    