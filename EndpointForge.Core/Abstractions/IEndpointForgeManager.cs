using EndpointForge.Core.Models;

namespace EndpointForge.Core.Abstractions;

public interface IEndpointForgeManager
{
    Task<IResult> TryAddEndpointAsync(AddEndpointRequest addEndpointRequest);
}