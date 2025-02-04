using EndpointForge.WebApi.Exceptions;

namespace EndpointForge.WebApi.Extensions;

public static class ReadOnlySpanExtensions
{
    public static bool IsStartOfPlaceholderEnd(this ReadOnlySpan<char> segmentSpan, int readPosition)
        => readPosition + 1 != segmentSpan.Length
           && segmentSpan[readPosition] is '}'
           && segmentSpan[readPosition + 1] is '}';

    public static bool IsStartOfPlaceholderBegin(this ReadOnlySpan<char> segmentSpan, int readPosition)
        => readPosition + 4 <= segmentSpan.Length
           && segmentSpan[readPosition] is '{'
           && segmentSpan[readPosition + 1] is '{'
           && !segmentSpan.IsStartOfPlaceholderEnd(readPosition + 2);

    public static ReadOnlySpan<char> ExtractPlaceholder(this ReadOnlySpan<char> segmentSpan, ref int readPosition)
    {
        var placeholderStartPosition = readPosition - 2;
        var placeholderContentStartPosition = readPosition;
        
        while (!segmentSpan.IsStartOfPlaceholderEnd(readPosition))
        {
            readPosition++;
            if (readPosition == segmentSpan.Length - 1)
                throw new InvalidPlaceholderException(
                    "Placeholder end marker not found",
                    placeholderStartPosition);
            if (segmentSpan.IsStartOfPlaceholderBegin(readPosition))
                throw new InvalidPlaceholderException(
                    $"New placeholder start marker found at position [{readPosition}] before the end of current placeholder",
                    placeholderStartPosition);
        }

        readPosition += 2;
        return segmentSpan[placeholderContentStartPosition..(readPosition-2)];
    }
}