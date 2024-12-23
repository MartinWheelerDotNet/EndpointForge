using Microsoft.AspNetCore.Http;

namespace EndpointManager.Abstractions.Interfaces;

public interface IEndpointManager
{
    Task<IResult> TryAddEndpointAsync(HttpRequest request);
}