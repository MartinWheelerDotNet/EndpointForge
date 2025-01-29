using EndpointForge.WebApi.Exceptions;

namespace EndpointForge.WebApi.Extensions;

public static class HttpRequestExtensions
{
    public static async Task<T> TryDeserializeRequestAsync<T>(
        this HttpRequest request) where T : class
    {
        if (request.ContentLength is null or 0)
            throw new BadRequestEndpointForgeException(["Request body must not be empty."]);
        try
        {
            await using var stream = new MemoryStream((int)request.Headers.ContentLength.GetValueOrDefault() + 1024);
            await request.Body.CopyToAsync(stream);
            var buffer = stream.GetBuffer();
            var bytesRead = (int)stream.Length;

            var jsonSpan = new ReadOnlySpan<byte>(buffer, 0, bytesRead);
            var reader = new Utf8JsonReader(jsonSpan);

            return JsonSerializer.Deserialize<T>(ref reader, JsonSerializerDefaults.EndpointForge)
                   ?? throw new InvalidRequestBodyEndpointForgeException();
        }
        catch (Exception exception)
        {
            throw new BadRequestEndpointForgeException([exception.Message]);
        }
    }
}