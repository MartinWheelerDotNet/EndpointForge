using EndpointForge.Abstractions;

namespace EndpointForge.WebApi.Tests.Fakes;

[ExcludeFromCodeCoverage]
internal class FakeGeneratorRule(string type = "", string value = "") : IEndpointForgeGeneratorRule
{
    public string Type => type;

    public bool TryInvoke(StreamWriter streamWriter, out ReadOnlySpan<char> generatedValue)
    {
        generatedValue = "";
        streamWriter.Write(value);
        return true;
    }
}