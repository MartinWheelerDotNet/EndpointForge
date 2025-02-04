namespace EndpointForge.WebApi.Extensions.Logger;

[ExcludeFromCodeCoverage]
internal static partial class LoggerExtensions
{
    private const string Start = "Start";
    private const string End = "End";
    
    [LoggerMessage(LogLevel.Information, "Processing Response Body: {Message}")]
    private static partial void LogResponseBodyParsing(this ILogger logger, string message);

    [LoggerMessage(LogLevel.Debug, "ResponseBody={ResponseBody}")]
    private static partial void LogResponseBody(this ILogger logger, ReadOnlySpan<char> responseBody);
    
    [LoggerMessage(LogLevel.Information, "Processing Segment ({SegmentName}: {Message}")]
    private static partial void LogSegmentParsing(
        this ILogger logger,
        ReadOnlySpan<char> segmentName,
        string message);

    [LoggerMessage(LogLevel.Debug, "Segment ({SegmentName})={ResponseBody}")]
    private static partial void LogSegment(this ILogger logger,
        ReadOnlySpan<char> segmentName,
        ReadOnlySpan<char> responseBody);
    

    public static void StartResponseBodyParsing(this ILogger logger, ReadOnlySpan<char> responseBody)
    {
        logger.LogResponseBodyParsing("Start");
        logger.LogResponseBody(responseBody);
    }

    public static void EndResponseBodyParsing(this ILogger logger)
        => logger.LogResponseBodyParsing("End");

    public static void StartSegmentParsing(
        this ILogger logger,
        ReadOnlySpan<char> segmentName,
        ReadOnlySpan<char> segment)
    {
        logger.LogSegmentParsing(segmentName, Start);
        logger.LogSegment(segmentName, segment);
    }
    
    public static void EndSegmentParsing(this ILogger logger, ReadOnlySpan<char> segmentName)
        => logger.LogSegmentParsing(segmentName, End);
}