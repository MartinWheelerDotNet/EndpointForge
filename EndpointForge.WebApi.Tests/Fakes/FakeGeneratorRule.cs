using System.Diagnostics.CodeAnalysis;
using EndpointForge.Core.Abstractions;

namespace EndpointForge.WebApi.Tests.Fakes;

[ExcludeFromCodeCoverage]
internal class FakeGeneratorRule(string type = "", string value = "") : IEndpointForgeGeneratorRule
{
    public string Instruction => "generate";
    public string Type => type;
    public void Invoke(StreamWriter streamWriter) => streamWriter.Write(value);
}