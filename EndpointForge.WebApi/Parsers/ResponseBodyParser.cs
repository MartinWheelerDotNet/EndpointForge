using EndpointForge.Abstractions.Interfaces;

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
        var lastWrittenIndex = 0;

        for (var readIndex = 0; readIndex < bodySpan.Length; readIndex++)
        {
            if (!IsStartOfPlaceholderBegin(readIndex, bodySpan))
                continue;
            
            streamWriter.Write(bodySpan[lastWrittenIndex..readIndex]);
            var startOfPlaceholderNameIndex = readIndex += 2;

            if (IsStartOfPlaceholderEnd(readIndex, bodySpan))
            {
                lastWrittenIndex = readIndex += 2;
                continue;
            }

            while (!IsStartOfPlaceholderEnd(readIndex, bodySpan))
                readIndex++;

            readIndex++;
            var placeholderName = bodySpan[startOfPlaceholderNameIndex..readIndex];

            if (!TryInvokeRule(streamWriter, placeholderName, parameters))
            {
                WritePlaceholder(streamWriter, placeholderName);
            }

            lastWrittenIndex = readIndex += 2;
        }
        
        streamWriter.Write(bodySpan[lastWrittenIndex..]);
        await streamWriter.FlushAsync();
        logger.LogInformation("Processed Response Body");
    }
 
    private bool TryInvokeRule(
        StreamWriter streamWriter,
        ReadOnlySpan<char> placeholderName,
        IDictionary<string, string> parameters)
    {
        var splitPlaceholder = placeholderName.Split(':');
        splitPlaceholder.MoveNext();
        
        var instruction = placeholderName[splitPlaceholder.Current];
        
        if (!splitPlaceholder.MoveNext())
            return false;
        
        var type = placeholderName[splitPlaceholder.Current];

        var parameterName = splitPlaceholder.MoveNext()
            ? placeholderName[splitPlaceholder.Current]
            : null;

        return instruction switch
        {
            "generate" => TryInvokeGeneratorRule(streamWriter, type),
            "insert" => TryInvokeInsertRule(streamWriter, type, parameterName, parameters),
            _ => false
        };
    }

    private bool TryInvokeGeneratorRule(StreamWriter writer, ReadOnlySpan<char> type)
    {
        var rule = ruleFactory.GetRule<IEndpointForgeGeneratorRule>(type);
        if (rule == null)
            return false;
            
        rule.Invoke(writer);
        return true;
    }

    private bool TryInvokeInsertRule(
        StreamWriter streamWriter,
        ReadOnlySpan<char> type,
        ReadOnlySpan<char> parameterName,
        IDictionary<string, string> parameters)
    {
        ReadOnlySpan<char> parameterValue = "";
        foreach (var (key, value) in parameters)
        {
            if (parameterName.SequenceEqual(key.AsSpan()))
                parameterValue = value.AsSpan();
        }

        if (parameterValue.IsEmpty)
        {
            streamWriter.Write("null");
            return true;
        }
        
        var rule = ruleFactory.GetRule<IEndpointForgeInsertRule>(type);
        if (rule == null)
            return false;
        
        
        rule.Invoke(streamWriter, parameterValue);
        return true;
    }
    
    private static void WritePlaceholder(StreamWriter writer, ReadOnlySpan<char> ruleName)
    {
        writer.Write("{{");
        writer.Write(ruleName);
        writer.Write("}}");
    }

    private static bool IsStartOfPlaceholderEnd(int index, ReadOnlySpan<char> bodySpan)
        => index + 1 < bodySpan.Length && bodySpan[index] == '}' || bodySpan[index + 1] == '}';

    private static bool IsStartOfPlaceholderBegin(int index, ReadOnlySpan<char> bodySpan)
        => index + 1 < bodySpan.Length && bodySpan[index] == '{' && bodySpan[index + 1] == '{';
}