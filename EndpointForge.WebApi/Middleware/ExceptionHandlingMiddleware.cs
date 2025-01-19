using System.Diagnostics;
using EndpointForge.WebApi.Exceptions;
using EndpointForge.WebApi.Extensions;
using EndpointForge.WebApi.Factories;

namespace EndpointForge.WebApi.Middleware;

public class ExceptionHandlingMiddleware(ILogger<ExceptionHandlingMiddleware> logger, RequestDelegate next)
{
    public async Task Invoke(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception exception)
        {
            context.Response.StatusCode = exception is EndpointForgeException endpointForgeException
                ? (int) endpointForgeException.StatusCode
                : (int) HttpStatusCode.InternalServerError;
            context.Response.Headers.Append("X-Trace-Id", Activity.Current?.TraceId.ToString());

            var errorResponse = ErrorResponseFactory.Create(exception);
            
            logger.LogErrorResponse(errorResponse);
            await context.Response.WriteAsJsonAsync(errorResponse);
        }
    }
}