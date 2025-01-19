using EndpointForge.Abstractions.Constants;
using EndpointForge.Models;
using EndpointForge.WebApi.Exceptions;

namespace EndpointForge.WebApi.Factories;

public static class ErrorResponseFactory
{
    public static ErrorResponse Create(Exception exception)
        => exception switch
        {
            BadRequestEndpointForgeException e => new ErrorResponse(
                ErrorStatusCode.InvalidRequestBody,
                exception.Message,
                e.Errors),
            ConflictEndpointForgeException e => new ErrorResponse(
                ErrorStatusCode.RouteConflict,
                exception.Message,
                e.Errors),
            InvalidRequestBodyEndpointForgeException e => new ErrorResponse(
                ErrorStatusCode.RequestBodyInvalidJson,
                exception.Message,
                e.Errors),
            EndpointForgeException e => new ErrorResponse(
                ErrorStatusCode.InternalServerError,
                exception.Message,
                e.Errors),
            _ => new ErrorResponse(
                ErrorStatusCode.InternalServerError,
                exception.Message,
                [])
        };
}