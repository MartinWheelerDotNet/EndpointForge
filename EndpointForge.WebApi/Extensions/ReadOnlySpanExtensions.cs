namespace EndpointForge.WebApi.Extensions;

public static class ReadOnlySpanExtensions
{
    public static bool IsStartOfPlaceholderEnd(this ReadOnlySpan<char> segmentSpan, int readPosition)
        => readPosition + 1 != segmentSpan.Length
           && segmentSpan[readPosition] is '}'
           && segmentSpan[readPosition + 1] is '}';

    public static bool IsStartOfPlaceholderBegin(this ReadOnlySpan<char> segmentSpan, int readPosition)
        => readPosition + 1 != segmentSpan.Length
           && segmentSpan[readPosition] is '{'
           && segmentSpan[readPosition + 1] is '{';

    public static ReadOnlySpan<char> ExtractPlaceholder(this ReadOnlySpan<char> segmentSpan, ref int readPosition)
    {
        var placeholderContentStartPosition = readPosition;
            
        // if the end of the placeholder is immediately after the placeholder (`{{}}`) then this is not a
        // valid placeholder and should be written.
        if (segmentSpan.IsStartOfPlaceholderEnd(readPosition))
        {
            // set the current read position to after the end of the placeholder.
            readPosition += 2;
            return ReadOnlySpan<char>.Empty;
        }
        
        while (!segmentSpan.IsStartOfPlaceholderEnd(readPosition))
        {
            readPosition++;
            if (readPosition == segmentSpan.Length - 1)
                throw new Exception("Placeholder end marker not found");
            if (segmentSpan.IsStartOfPlaceholderBegin(readPosition))
                throw new Exception("New placeholder start marker found before end of current placeholder");
        }
        
        var placeholder = segmentSpan[placeholderContentStartPosition..readPosition];

        //set current read position to the first character after the placeholder end marker
        readPosition += 2;

        return placeholder;
    }
}