using System.Buffers;
using EndpointForge.Abstractions.Interfaces;

namespace EndpointForge.WebApi.Writers;

public class GuidWriter : IGuidWriter
{
    private const int GuidCharSize = 36;
    private static readonly ArrayPool<char> CharPool = ArrayPool<char>.Shared;
    public void WriteGuidToStream(Guid guid, StreamWriter streamWriter)
    {
        var guidCharBuffer = CharPool.Rent(GuidCharSize);

        try
        {
            guid.TryFormat(guidCharBuffer, out _);
            streamWriter.Write(guidCharBuffer, 0, GuidCharSize);
        }
        finally
        {
            CharPool.Return(guidCharBuffer);
        }
    }
}