namespace EndpointForge.Abstractions.Interfaces;

public interface IGuidWriter
{
    void WriteGuidToStream(Guid guid, StreamWriter streamWriter);
}