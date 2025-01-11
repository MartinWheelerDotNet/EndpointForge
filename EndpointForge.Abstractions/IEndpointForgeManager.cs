using EndpointForge.Models;

namespace EndpointForge.Abstractions;

public interface IEndpointForgeManager
{
    Task<IResult> TryAddEndpointAsync(AddEndpointRequest addEndpointRequest);
}