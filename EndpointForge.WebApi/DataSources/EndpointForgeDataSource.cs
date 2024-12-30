using System.Net.Mime;
using EndpointForge.Abstractions.Extensions;
using EndpointForge.Abstractions.Interfaces;
using EndpointForge.Abstractions.Models;
using Microsoft.AspNetCore.Routing.Patterns;
using static System.Text.Encoding;

namespace EndpointForge.WebApi.DataSources;

public class EndpointForgeDataSource : MutableEndpointDataSource, IEndpointForgeDataSource
{
    public void AddEndpoint(AddEndpointRequest addEndpointRequest, bool apply = true)
    {
        var endpoint = new RouteEndpointBuilder(
                BuildResponse(addEndpointRequest.Response), 
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
            context.Response.ContentType = GetContentType(responseDetails);

            if (!string.IsNullOrWhiteSpace(responseDetails.Body))
            {
                context.Response.ContentLength = UTF8.GetByteCount(responseDetails.Body!);
                await context.Response.WriteAsync(responseDetails.Body);
            }
        };
    
    private static string? GetContentType(EndpointResponseDetails responseDetails)
        => (responseDetails.HasBody(), responseDetails.HasContentType()) switch 
        {
            (false, _) => null,
            (true, false) => MediaTypeNames.Text.Plain,
            (true, true) => responseDetails.ContentType
        };
}