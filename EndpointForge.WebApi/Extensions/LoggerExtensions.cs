namespace EndpointForge.WebApi.Extensions;

[ExcludeFromCodeCoverage]
internal static partial class LoggerExtensions
{
    #region Error Response

    [LoggerMessage(LogLevel.Debug, "{ErrorResponse}")]
    private static partial void ErrorResponse(
        this ILogger logger,
        [LogProperties(OmitReferenceName = true)]
        in Models.ErrorResponse errorResponse);

    [LoggerMessage(LogLevel.Information, "An Error Response has been returned: [{statusCode}] {statusDescription}")]
    public static partial void ErrorResponseInformation(
        this ILogger logger,
        int statusCode,
        HttpStatusCode statusDescription);

    public static void LogErrorResponse(this ILogger logger, Models.ErrorResponse errorResponse)
    {
        logger.ErrorResponseInformation((int)errorResponse.StatusCode, errorResponse.StatusCode);
        logger.ErrorResponse(errorResponse);
    }

    #endregion

    [LoggerMessage(LogLevel.Debug, "{AddEndpointRequest}")]
    private static partial void AddEndpointRequest(
        this ILogger logger,
        [LogProperties(OmitReferenceName = true)] in Models.AddEndpointRequest addEndpointRequest);

    [LoggerMessage(LogLevel.Information, "Endpoint Created: {Details}")]
    private static partial void EndpointRoutingDetails(
        this ILogger logger,
        [LogProperties(OmitReferenceName = true)] in Models.EndpointRoutingDetails details);

    public static void LogAddEndpointRequestCompleted(this ILogger logger, Models.AddEndpointRequest addEndpointRequest)
    {
        foreach (var details in addEndpointRequest.GetEndpointRoutingDetails())
        {
            logger.EndpointRoutingDetails(details); 
        }
        logger.AddEndpointRequest(addEndpointRequest);
    }
}
