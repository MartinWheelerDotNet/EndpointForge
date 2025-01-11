
using System.Net.Mime;
using EndpointForge.Abstractions;
using EndpointForge.Models;
using EndpointForge.WebApi.Extensions;

namespace EndpointForge.WebApi.Builders;

public class RequestDelegateBuilder(
    ILogger<RequestDelegateBuilder> logger,
    IResponseBodyParser bodyParser,
    IParameterProcessor parameterProcessor,
    RecyclableMemoryStreamManager memoryStreamManager) : IRequestDelegateBuilder
{
    public RequestDelegate BuildResponse(
        EndpointResponseDetails responseDetails,
        List<EndpointParameterDetails> parameters)
    {
        return async context =>
        {
            logger.LogInformation("Building response headers");
            context.Response.StatusCode = responseDetails.StatusCode;
            context.Response.ContentType = GetContentType(responseDetails);

            logger.LogInformation("Building response body and writing the response body");
            if (!string.IsNullOrWhiteSpace(responseDetails.Body))
            {
                await using var memoryStream = memoryStreamManager.GetStream();
                var processedParameters = parameterProcessor.Process(parameters, context);
                await bodyParser.ProcessResponseBody(memoryStream, responseDetails.Body, processedParameters);

                memoryStream.Seek(0, SeekOrigin.Begin);

                context.Response.ContentLength = memoryStream.Length;
                await memoryStream.CopyToAsync(context.Response.Body);
            }

            logger.LogInformation("Response fully written.");
        };
    }

    private static string? GetContentType(EndpointResponseDetails responseDetails)
        => (responseDetails.HasBody(), responseDetails.HasContentType()) switch
        {
            (true, false) => MediaTypeNames.Text.Plain,
            (true, true) => responseDetails.ContentType,
            _ => null
        };
    
}
