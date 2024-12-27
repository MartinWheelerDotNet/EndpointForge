using EndpointManager.Abstractions.Models;
using Microsoft.AspNetCore.Routing.Patterns;

namespace EndpointForge.WebApi.Models;

public class EndpointForgeDataSource : MutableEndpointDataSource, IEndpointForgeDataSource
{
    public void AddEndpoint(AddEndpointRequest addEndpointRequest, bool apply = true)
    {
        var (pattern, _, priority) = addEndpointRequest;
        var endpoint = new RouteEndpointBuilder(BuildResponse, RoutePatternFactory.Parse(pattern), priority)
            {
                Metadata = { new HttpMethodMetadata([addEndpointRequest.Method]) }
            }
            .Build();
        base.AddEndpoint(endpoint, apply);
    }

    private static RequestDelegate BuildResponse => async _ => await Task.FromResult(TypedResults.Ok());
}