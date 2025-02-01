using EndpointForge.Abstractions;
using EndpointForge.WebApi.Constants;
using EndpointForge.WebApi.Extensions;
using EndpointForge.WebApi.Models;

namespace EndpointForge.WebApi.Parsers;

public class ResponseBodyParser(
    ILogger<ResponseBodyParser> logger,
    IEndpointForgeRuleFactory ruleFactory) : IResponseBodyParser
{
    public async Task ProcessResponseBody(
        Stream stream,
        string responseBody,
        Dictionary<string, string> parameters)
    {
        logger.LogInformation("Processing Response Body");
        var streamWriter = new StreamWriter(stream);
        var bodySpan = responseBody.AsSpan();
        var lastWrittenPosition = 0;

        // process the whole response body as segment
        ProcessSegment(streamWriter, bodySpan, ref lastWrittenPosition, parameters);

        // flush the stream writer to write the buffered data and clear buffers
        await streamWriter.FlushAsync();
        logger.LogInformation("Processed Response Body");
    }

    private void ProcessSegment(
        StreamWriter streamWriter,
        ReadOnlySpan<char> segmentSpan,
        ref int lastWritePosition,
        Dictionary<string, string> parameters)
    {
        for (var currentReadPosition = 0; currentReadPosition < segmentSpan.Length; currentReadPosition++)
        {
            // if start of placeholder is not found loop until first placeholder is found
            if (!segmentSpan.IsStartOfPlaceholderBegin(currentReadPosition))
                continue;

            // write data from the last write position of the span to the current read position
            // and set the last write position to this index.
            streamWriter.Write(segmentSpan[lastWritePosition..currentReadPosition]);
            lastWritePosition = currentReadPosition;
            
            // skip placeholder markers and record position of placeholder content
            currentReadPosition += 2;
            
            var placeholder = segmentSpan.ExtractPlaceholder(ref currentReadPosition);
            
            if (!PlaceholderDetails.TryParse(placeholder, out var placeholderDetails))
            {
                logger.LogInformation($"Placeholder: {placeholder}");

                streamWriter.Write(segmentSpan[lastWritePosition..currentReadPosition]);
                lastWritePosition = currentReadPosition;
                continue;   
            }
            
            // if the placeholder is for the start of a repeater
            if (placeholderDetails is { Rule: RuleType.Repeat, Instruction: InstructionType.Start })
            {
                
                // set the last Write Position and current read position to end of the placeholder end ('}}')
                lastWritePosition = currentReadPosition;
                
                if (!int.TryParse(placeholderDetails.Values, out var repeaterAmount))
                    throw new FormatException("Repeater value is not a number");

                var repeaterSegmentSpan = ExtractRepeaterSegment(
                    segmentSpan,
                    placeholderDetails.Name,
                    currentReadPosition,
                    out var readOffset);
                
                for (var count = 0; count < repeaterAmount; count++)
                {
                    var segmentReadPosition = 0;
                    ProcessSegment(streamWriter, repeaterSegmentSpan, ref segmentReadPosition, parameters);
                }
                
                currentReadPosition = lastWritePosition = readOffset;
                continue;
            }
            
            logger.LogInformation("I'm not a repeater");
            
            if (!TryInvokeRule(streamWriter, placeholderDetails, parameters))
                WritePlaceholder(streamWriter, placeholder);

            lastWritePosition = currentReadPosition;
        }
        
        streamWriter.Write(segmentSpan[lastWritePosition..]);
    }

    private ReadOnlySpan<char> ExtractRepeaterSegment(
        ReadOnlySpan<char> segmentSpan,
        ReadOnlySpan<char> repeaterName,
        int segmentStartPosition,
        out int readOffset)
    {
        for (readOffset = segmentStartPosition; readOffset < segmentSpan.Length; readOffset++)
        {
            if (!segmentSpan.IsStartOfPlaceholderBegin(readOffset))
                continue;

            var segmentEndPosition = readOffset;
            var placeholderContentStartPosition = readOffset += 2;

            if (segmentSpan.IsStartOfPlaceholderEnd(readOffset))
                continue;
            
            while (!segmentSpan.IsStartOfPlaceholderEnd(readOffset))
            {
                if (readOffset + 1 == segmentSpan.Length)
                    throw new FormatException("Placeholder end not found");
                
                readOffset++;
            }

            var placeholder = segmentSpan[placeholderContentStartPosition..readOffset];
            if (!PlaceholderDetails.TryParse(placeholder, out var placeholderDetails))
                continue;   
            
            readOffset += 2;

            if (placeholderDetails.IsMatchingRepeatEnd(repeaterName))
                return segmentSpan[segmentStartPosition..segmentEndPosition];
        }
        throw new ApplicationException("Could not find repeater end");
    }

    private bool TryInvokeRule(
        StreamWriter streamWriter,
        PlaceholderDetails placeholderDetails,
        IDictionary<string, string> parameters)
    {
        
        logger.LogInformation($"invoke rule: {placeholderDetails.Instruction}");
        return placeholderDetails.Rule switch
        {
            RuleType.Generate => TryInvokeGeneratorRule(streamWriter, placeholderDetails.Instruction, placeholderDetails.Name, parameters),
            RuleType.Insert => TryInvokeInsertRule(streamWriter,placeholderDetails.Instruction, placeholderDetails.Name, parameters),
            _ => false
        };
    }

    private bool TryInvokeGeneratorRule(
        StreamWriter writer,
        ReadOnlySpan<char> type,
        ReadOnlySpan<char> parameterName,
        IDictionary<string, string> parameters)
    {
        var rule = ruleFactory.GetRule<IEndpointForgeGeneratorRule>(type);

        if (rule is null || !rule.TryInvoke(writer, out var generatedValue))
            return false;

        if (parameterName is not "")
            parameters.Add(parameterName.ToString(), generatedValue.ToString());
        return true;
    }

    private bool TryInvokeInsertRule(
        StreamWriter streamWriter,
        ReadOnlySpan<char> type,
        ReadOnlySpan<char> parameterName,
        IDictionary<string, string> parameters)
    {
        logger.LogInformation("Start insert rule");
        ReadOnlySpan<char> parameterValue = "";
        foreach (var (key, value) in parameters)
        {
            if (parameterName.SequenceEqual(key.AsSpan()))
                parameterValue = value.AsSpan();
        }

        if (parameterValue.IsEmpty)
        {
            
            logger.LogInformation("No parameter found.");
            return false;
        }

        logger.LogInformation("Parameter found");
        var rule = ruleFactory.GetRule<IEndpointForgeInsertRule>(type);
        if (rule == null)
            return false;
        
        
        logger.LogInformation($"rule found: {rule}");


        rule.Invoke(streamWriter, parameterValue);
        return true;
    }

    private void WritePlaceholder(StreamWriter writer, ReadOnlySpan<char> placeholder)
    {
        logger.LogInformation($"Writing placeholder to stream: {placeholder}");
        writer.Write("{{");
        writer.Write(placeholder);
        writer.Write("}}");
    }
}