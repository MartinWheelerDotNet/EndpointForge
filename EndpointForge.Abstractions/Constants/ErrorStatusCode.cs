namespace EndpointForge.Abstractions.Constants;

public readonly struct ErrorStatusCode
{
    public const string InvalidRequestBody = "INVALID_REQUEST_BODY";
    public const string RequestBodyInvalidJson = "REQUEST_BODY_INVALID_JSON";
    public const string RouteConflict = "ROUTE_CONFLICT";
    public const string InternalServerError = "UNKNOWN_SERVER_ERROR";
}