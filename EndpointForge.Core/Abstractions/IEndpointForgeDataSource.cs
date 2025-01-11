using EndpointForge.Core.Models;

namespace EndpointForge.Core.Abstractions;

public interface IEndpointForgeDataSource
{
    void AddEndpoint(AddEndpointRequest addEndpointRequest, bool apply = true);
}