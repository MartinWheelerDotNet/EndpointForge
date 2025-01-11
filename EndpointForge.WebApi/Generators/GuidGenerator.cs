using EndpointForge.Abstractions.Interfaces;

namespace EndpointForge.WebApi.Generators;

public class GuidGenerator : IGuidGenerator
{
    public Guid New => Guid.NewGuid();
}