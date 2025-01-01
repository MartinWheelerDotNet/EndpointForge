using System.Diagnostics.CodeAnalysis;
using EndpointForge.Abstractions.Interfaces;

namespace EndpointForge.WebApi.Tests.Fakes;

[ExcludeFromCodeCoverage]
internal class FakeGuidGenerator(Guid guid) : IGuidGenerator
{
    public Guid New() => guid;
}