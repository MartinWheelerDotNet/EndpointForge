using System.Diagnostics.CodeAnalysis;
using EndpointForge.Abstractions.Interfaces;
using EndpointForge.Abstractions.Models;

namespace EndpointForge.WebApi.Tests.Fakes;

[ExcludeFromCodeCoverage]
public class FakeEndpointForgeDataSource : IEndpointForgeDataSource
{
    public List<AddEndpointRequest> AddedEndpoints { get; } = [];

    public void AddEndpoint(AddEndpointRequest addEndpointRequest, bool apply = true)
    {
        AddedEndpoints.Add(addEndpointRequest);
    }
}