using EndpointForge.Models;

namespace EndpointForge.Abstractions;

public interface IEndpointForgeDataSource
{
    void AddEndpoint(AddEndpointRequest addEndpointRequest, bool apply = true);
}