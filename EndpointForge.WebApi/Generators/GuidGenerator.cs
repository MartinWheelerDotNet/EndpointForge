using EndpointForge.Abstractions;

namespace EndpointForge.WebApi.Generators;

public class GuidGenerator : IGuidGenerator
{
    public Guid New => Guid.NewGuid();
}