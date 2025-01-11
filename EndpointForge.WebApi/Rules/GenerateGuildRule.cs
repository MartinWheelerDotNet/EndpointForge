using EndpointForge.Abstractions;

namespace EndpointForge.WebApi.Rules;

public class GenerateGuidRule(IGuidGenerator guidGenerator) : IEndpointForgeGeneratorRule
{
    private const string RuleInstruction = "generate";
    private const string RuleType = "guid";
    public string Instruction => RuleInstruction;
    public string Type => RuleType;
    

    private const int GuidCharSize = 36;
    private static readonly ArrayPool<char> CharPool = ArrayPool<char>.Shared;

    public void Invoke(StreamWriter streamWriter)
    {
        var guidCharBuffer = CharPool.Rent(GuidCharSize);

        try
        {
            guidGenerator.New.TryFormat(guidCharBuffer, out _);
            streamWriter.Write(guidCharBuffer, 0, GuidCharSize);
        }
        finally
        {
            CharPool.Return(guidCharBuffer);
        }
    }
}