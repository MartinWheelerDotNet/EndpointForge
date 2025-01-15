using EndpointForge.Abstractions;

namespace EndpointForge.WebApi.Tests.Fakes;

[ExcludeFromCodeCoverage]
internal class FakeGeneratorRule(string type = "", string value = "") : IEndpointForgeGeneratorRule
{
    public string Type => type;
    public void Invoke(StreamWriter streamWriter) => streamWriter.Write(value);
}