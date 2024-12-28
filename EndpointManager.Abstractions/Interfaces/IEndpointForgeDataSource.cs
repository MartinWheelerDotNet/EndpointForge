using EndpointManager.Abstractions.Models;

namespace EndpointManager.Abstractions.Interfaces;

public interface IEndpointForgeDataSource
{
    void AddEndpoint(AddEndpointRequest addEndpointRequest, bool apply = true);
}