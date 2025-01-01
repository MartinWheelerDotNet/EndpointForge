using System.Net.Mime;
using EndpointForge.Abstractions.Extensions;
using EndpointForge.Abstractions.Interfaces;
using EndpointForge.Abstractions.Models;
using Microsoft.AspNetCore.Routing.Patterns;
using Microsoft.IO;

namespace EndpointForge.WebApi.DataSources;

public class EndpointForgeDataSource(
    ILogger<EndpointForgeDataSource> logger,
    IResponseBodyParser bodyParser,
    RecyclableMemoryStreamManager memoryStreamManager) : MutableEndpointDataSource, IEndpointForgeDataSource
{
    public void AddEndpoint(AddEndpointRequest addEndpointRequest, bool apply = true)
    {
        var endpoint = new RouteEndpointBuilder(
                BuildResponse(addEndpointRequest.Response),
                RoutePatternFactory.Parse(addEndpointRequest.Route),
                0)
            {
                Metadata = { new HttpMethodMetadata(addEndpointRequest.Methods) }
            }
            .Build();
        base.AddEndpoint(endpoint, apply);
    }

    private RequestDelegate BuildResponse(EndpointResponseDetails responseDetails)
        => async context =>
        {
            logger.LogInformation("Building response headers");
            context.Response.StatusCode = responseDetails.StatusCode;
            context.Response.ContentType = GetContentType(responseDetails);

            logger.LogInformation("Building response body and writing the response body");
            if (!string.IsNullOrWhiteSpace(responseDetails.Body))
            {
                await using var memoryStream = memoryStreamManager.GetStream(responseDetails.Body);
                await bodyParser.ProcessResponseBody(memoryStream, responseDetails.Body);
                context.Response.ContentLength = memoryStream.Length;
                
                memoryStream.Seek(0, SeekOrigin.Begin);
                await memoryStream.CopyToAsync(context.Response.Body);
            }
            logger.LogInformation("Response fully written.");
        };


    private static string? GetContentType(EndpointResponseDetails responseDetails)
        => (responseDetails.HasBody(), responseDetails.HasContentType()) switch
        {
            (false, _) => null,
            (true, false) => MediaTypeNames.Text.Plain,
            (true, true) => responseDetails.ContentType
        };
}