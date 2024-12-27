using EndpointManager.Abstractions.Models;
using Microsoft.AspNetCore.Routing.Patterns;

namespace EndpointForge.WebApi.Models;

public class EndpointForgeDataSource : MutableEndpointDataSource, IEndpointForgeDataSource
{
    public void AddEndpoint(AddEndpointRequest addEndpointRequest, bool apply = true)
    {
        var (pattern, method, response, priority) = addEndpointRequest;
        //var response = addEndpointRequest.Response ?? new EndpointRequestResponse(200);
        var endpoint = new RouteEndpointBuilder(
                BuildResponse(response ?? new EndpointRequestResponse(200)), 
                RoutePatternFactory.Parse(pattern), 
                priority)
            {
                Metadata = { new HttpMethodMetadata([method]) }
            }
            .Build();
        base.AddEndpoint(endpoint, apply);
    }

    private static RequestDelegate BuildResponse(EndpointRequestResponse response)
        => async context =>
        {
            context.Response.StatusCode = response.StatusCode;
            await Task.CompletedTask; // Simulate async operation for now
        };
}