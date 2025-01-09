using EndpointForge.Abstractions.Interfaces;
using EndpointForge.Abstractions.Models;
using Microsoft.AspNetCore.Routing.Patterns;

namespace EndpointForge.WebApi.DataSources;

public class EndpointForgeDataSource(
    ILogger<EndpointForgeDataSource> logger,
    IRequestDelegateBuilder requestDelegateBuilder) : MutableEndpointDataSource(logger), IEndpointForgeDataSource
{
    public void AddEndpoint(AddEndpointRequest addEndpointRequest, bool apply = true)
    {
        var endpoint = new RouteEndpointBuilder(
                requestDelegateBuilder.BuildResponse(addEndpointRequest.Response, addEndpointRequest.Parameters.ToList()),
                RoutePatternFactory.Parse(addEndpointRequest.Route), 0)
            {
                Metadata = { new HttpMethodMetadata(addEndpointRequest.Methods) }
            }
            .Build();
        base.AddEndpoint(endpoint, apply);
    }
}