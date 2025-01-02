using EndpointForge.Abstractions.Interfaces;

namespace EndpointForge.Abstractions.Generators;

public class GuidGenerator : IGuidGenerator
{
    public Guid New => Guid.NewGuid();
}