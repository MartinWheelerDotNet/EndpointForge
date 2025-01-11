using System.Diagnostics.CodeAnalysis;
using EndpointForge.Core.Abstractions;

namespace EndpointForge.WebApi.Tests.Fakes;

[ExcludeFromCodeCoverage]
internal class FakeInsertRule(string type = "") : IEndpointForgeInsertRule
{
    public string Instruction => "insert";
    public string Type => type;

    public void Invoke(StreamWriter streamWriter, ReadOnlySpan<char> value) => streamWriter.Write(value);
}