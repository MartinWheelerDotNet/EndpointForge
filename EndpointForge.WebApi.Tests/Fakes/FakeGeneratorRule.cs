using System.Diagnostics.CodeAnalysis;
using EndpointForge.Abstractions.Interfaces;

namespace EndpointForge.WebApi.Tests.Fakes;

[ExcludeFromCodeCoverage]
internal class FakeGeneratorRule(string placeholder = "", string content = "") : IEndpointForgeGeneratorRule
{
    public string Placeholder => placeholder;
    public void Invoke(StreamWriter streamWriter) => streamWriter.Write(content);
}