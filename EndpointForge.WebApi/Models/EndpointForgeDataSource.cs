using EndpointManager.Abstractions.Interfaces;
using EndpointManager.Abstractions.Models;
using Microsoft.AspNetCore.Routing.Patterns;

namespace EndpointForge.WebApi.Models;

public class EndpointForgeDataSource : MutableEndpointDataSource, IEndpointForgeDataSource
{
    public void AddEndpoint(AddEndpointRequest addEndpointRequest, bool apply = true)
    {
        var endpoint = new RouteEndpointBuilder(
                BuildResponse(addEndpointRequest.Response ?? new EndpointResponseDetails(200)), 
                RoutePatternFactory.Parse(addEndpointRequest.Route), 
                addEndpointRequest.Priority)
            {
                Metadata = { new HttpMethodMetadata(addEndpointRequest.Methods) }
            }
            .Build();
        base.AddEndpoint(endpoint, apply);
    }

    private static RequestDelegate BuildResponse(EndpointResponseDetails responseDetails)
        => async context =>
        {
            context.Response.StatusCode = responseDetails.StatusCode;
            await Task.CompletedTask; // Simulate async operation for now
        };
}