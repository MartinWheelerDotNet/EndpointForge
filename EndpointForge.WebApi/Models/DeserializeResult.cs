using EndpointManager.Abstractions.Models;

namespace EndpointForge.WebApi.Models;

public record DeserializeResult<T>(T? Result = null, ErrorResponse? ErrorResponse = null) where T : class;