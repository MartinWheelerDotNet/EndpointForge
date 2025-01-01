using EndpointForge.Abstractions.Interfaces;

namespace EndpointForge.WebApi.Parsers;

public class ResponseBodyParser(IEndpointForgeRuleFactory ruleFactory) : IResponseBodyParser
{
    public async Task ProcessResponseBody(Stream stream, string responseBody)
    {
        var writer = new StreamWriter(stream);
        
        var bodySpan = responseBody.AsSpan();

        var lastWrittenIndex = 0;

        for (var readIndex = 0; readIndex < bodySpan.Length; readIndex++)
        {
            if (!IsStartOfPlaceholderBegin(readIndex, bodySpan))
                continue;
            
            writer.Write(bodySpan[lastWrittenIndex..readIndex]);

            readIndex += 2;
            
            var startOfPlaceholderNameIndex = readIndex;

            if (IsStartOfPlaceholderEnd(readIndex, bodySpan))
            {
                WriteSubstitutionRuleResultOrPlaceHolder(writer, "");
                lastWrittenIndex = readIndex += 2;
                continue;
            }

            while (!IsStartOfPlaceholderEnd(readIndex, bodySpan))
                readIndex++;

            var endOfPlaceholderNameIndex = readIndex + 1;

            WriteSubstitutionRuleResultOrPlaceHolder(
                writer,
                bodySpan[startOfPlaceholderNameIndex..endOfPlaceholderNameIndex]);
            
            readIndex += 3;
            lastWrittenIndex = readIndex;
        }

        
        writer.Write(bodySpan[lastWrittenIndex..]);
        await writer.FlushAsync();
    }

    private void WriteSubstitutionRuleResultOrPlaceHolder(StreamWriter writer, ReadOnlySpan<char> placeholderName)
    {
        if (placeholderName.IsEmpty)
        {
                writer.Write("{{}}");
                return;   
        }
        
        var splitPlaceholder = placeholderName.Split(':');
        splitPlaceholder.MoveNext();
        
        var instruction = placeholderName[splitPlaceholder.Current];
        
        if (!splitPlaceholder.MoveNext())
        {
            WritePlaceholder(writer,placeholderName);
            return;
        }
        
        if (instruction.SequenceEqual("generate".AsSpan()))
        {
            var rule = ruleFactory.GetGeneratorRule(placeholderName);
            if (rule == null)
            {
                WritePlaceholder(writer, placeholderName);
                return;
            }
            
            rule.Invoke(writer);
        }
        else
        {
            WritePlaceholder(writer, placeholderName);
        }
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