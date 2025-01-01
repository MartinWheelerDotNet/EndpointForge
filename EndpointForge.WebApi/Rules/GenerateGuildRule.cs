using EndpointForge.Abstractions.Interfaces;

namespace EndpointForge.WebApi.Rules;

public class GenerateGuidRule(IGuidGenerator guidGenerator, IGuidWriter guidWriter) : IEndpointForgeGeneratorRule
{
    private const string RulePlaceholder = "generate:guid";
    public string Placeholder => RulePlaceholder;

    public void Invoke(StreamWriter streamWriter)
    {
        guidWriter.WriteGuidToStream(guidGenerator.New(), streamWriter);
    } 
}