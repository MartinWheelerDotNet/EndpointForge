using Microsoft.VisualBasic;

namespace EndpointForge.WebApi;

public static class JsonSerializerDefaults
{
    public static JsonSerializerOptions EndpointForge { get; } = new()
    {
        DefaultBufferSize = 128 * 1024,
        PropertyNameCaseInsensitive = true,
    };
}