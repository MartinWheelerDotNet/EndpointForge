using System.Net;
using EndpointForge.Abstractions.Models;

namespace EndpointForge.WebApi.Extensions;

public static class ErrorResultExtensions
{
    public static IResult GetTypedResult(this ErrorResponse errorResponse)
        => errorResponse.StatusCode switch
        {
            HttpStatusCode.UnprocessableEntity => TypedResults.UnprocessableEntity(errorResponse),
            HttpStatusCode.Conflict => TypedResults.Conflict(errorResponse),
            _ => TypedResults.BadRequest(errorResponse)
        };
}