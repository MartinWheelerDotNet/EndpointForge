using EndpointForge.Abstractions;
using EndpointForge.WebApi.Constants;

namespace EndpointForge.WebApi.Rules;

public class GenerateGuidRule(IGuidGenerator guidGenerator) : IEndpointForgeGeneratorRule
{
    public string Type => InstructionType.Guid;
    

    private const int GuidCharSize = 36;
    private static readonly ArrayPool<char> CharPool = ArrayPool<char>.Shared;

    public bool TryInvoke(StreamWriter streamWriter, out ReadOnlySpan<char> generatedValue)
    {
        var guidCharBuffer = CharPool.Rent(GuidCharSize);

        try
        {
            guidGenerator.New.TryFormat(guidCharBuffer, out _);
            streamWriter.Write(guidCharBuffer, 0, GuidCharSize);
            generatedValue = guidCharBuffer.AsSpan(0, GuidCharSize);
            return true;
        }
        catch
        {
            generatedValue = "";
            return false;
        }
        finally
        {
            CharPool.Return(guidCharBuffer);
        }
    }
}