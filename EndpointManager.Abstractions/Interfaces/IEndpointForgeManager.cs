using Microsoft.AspNetCore.Http;

namespace EndpointManager.Abstractions.Interfaces;

public interface IEndpointForgeManager
{
    Task<IResult> TryAddEndpointAsync(HttpRequest request);
}