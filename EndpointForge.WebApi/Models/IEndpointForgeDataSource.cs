using EndpointManager.Abstractions.Models;

namespace EndpointForge.WebApi.Models;

public interface IEndpointForgeDataSource
{
    void AddEndpoint(AddEndpointRequest addEndpointRequest, bool apply = true);
}