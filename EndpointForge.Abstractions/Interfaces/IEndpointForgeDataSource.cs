using EndpointForge.Abstractions.Models;

namespace EndpointForge.Abstractions.Interfaces;

public interface IEndpointForgeDataSource
{
    void AddEndpoint(AddEndpointRequest addEndpointRequest, bool apply = true);
}