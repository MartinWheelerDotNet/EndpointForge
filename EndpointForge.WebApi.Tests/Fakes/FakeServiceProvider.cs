using System.Diagnostics.CodeAnalysis;

namespace EndpointForge.WebApi.Tests.Fakes;

[ExcludeFromCodeCoverage]
internal class FakeServiceProvider(object? service) : IServiceProvider
{
    public object? GetService(Type serviceType)
    {
        return service;
    }
}