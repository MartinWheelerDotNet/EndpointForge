using EndpointForge.Abstractions.Exceptions;
using EndpointForge.Abstractions.Models;
using EndpointForge.WebApi.Extensions;

namespace EndpointForge.WebApi.Middleware;

public class ExceptionHandlingMiddleware(ILogger<ExceptionHandlingMiddleware> logger,RequestDelegate next)
{
    public async Task Invoke(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (EndpointForgeException exception)
        {
            await HandleExceptionAsync(context, exception);
        }
        catch(Exception exception)
        {
            await HandleExceptionAsync(
                context,
                new BadRequestEndpointForgeException([exception.Message]));
        }
    }

    private Task HandleExceptionAsync(HttpContext context, EndpointForgeException exception)
    {
        context.Response.StatusCode = (int) exception.StatusCode;
        var errorResponse = new ErrorResponse(exception.StatusCode, exception.Message, exception.Errors);

        logger.LogErrorResponse(errorResponse);
        return context.Response.WriteAsJsonAsync(errorResponse);
    }
}