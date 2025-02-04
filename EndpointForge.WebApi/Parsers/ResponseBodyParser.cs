using EndpointForge.Abstractions;
using EndpointForge.WebApi.Constants;
using EndpointForge.WebApi.Extensions;
using EndpointForge.WebApi.Extensions.Logger;
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
        logger.StartResponseBodyParsing(responseBody);
        
        var streamWriter = new StreamWriter(stream);
        var bodySpan = responseBody.AsSpan();
        var lastWrittenPosition = 0;

        ProcessSegment(streamWriter, bodySpan, "body", ref lastWrittenPosition, parameters);

        await streamWriter.FlushAsync();
        logger.EndResponseBodyParsing();
    }

    private void ProcessSegment(
        StreamWriter streamWriter,
        ReadOnlySpan<char> segmentSpan,
        ReadOnlySpan<char> segmentName,
        ref int lastWritePosition,
        Dictionary<string, string> parameters)
    {
        logger.StartSegmentParsing(segmentName, segmentSpan);
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
                streamWriter.Write(segmentSpan[lastWritePosition..currentReadPosition]);
                lastWritePosition = currentReadPosition;
                continue;   
            }
            
            // if the placeholder is for the start of a repeater
            if (placeholderDetails.IsRepeatStart())
            {
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
                    ProcessSegment(streamWriter, repeaterSegmentSpan, placeholderDetails.Name, ref segmentReadPosition, parameters);
                }
                
                currentReadPosition = readOffset;
                lastWritePosition = currentReadPosition - 2;
                continue;
            }
            
            if (!TryInvokeRule(streamWriter, placeholderDetails, parameters))
                WritePlaceholder(streamWriter, placeholder);

            lastWritePosition = currentReadPosition;
        }
        streamWriter.Write(segmentSpan[lastWritePosition..]);
        logger.EndSegmentParsing(segmentName);
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
            
            var placeholderEndPosition = readOffset;
            
            readOffset += 2;
            
            var placeholderContent = segmentSpan.ExtractPlaceholder(ref readOffset);
            
            if (!PlaceholderDetails.TryParse(placeholderContent, out var placeholderDetails))
                continue;   
            
            readOffset += 2;

            if (placeholderDetails.IsMatchingRepeatEnd(repeaterName))
                return segmentSpan[segmentStartPosition..placeholderEndPosition];
        }
        throw new ApplicationException("Could not find repeater end");
    }

    private bool TryInvokeRule(
        StreamWriter streamWriter,
        PlaceholderDetails placeholderDetails,
        IDictionary<string, string> parameters)
    {
        
        logger.LogInformation($"Invoke Rule: {placeholderDetails.Instruction}");
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

        logger.LogInformation($"Invoking rule: {rule.Type}");

        if (!parameterName.IsEmpty)
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