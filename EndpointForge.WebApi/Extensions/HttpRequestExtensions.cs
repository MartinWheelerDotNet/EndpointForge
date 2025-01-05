using System.Text;
using System.Text.Json;
using EndpointForge.Abstractions.Exceptions;

namespace EndpointForge.WebApi.Extensions;

public static class HttpRequestExtensions
{
    public static async Task WriteAsJsonAsync<T>(this HttpRequest request, T data)
    {
        request.ContentType = "application/json";
        
        var json = JsonSerializer.Serialize(data);
        var bytes = Encoding.UTF8.GetBytes(json);

        request.Body = new MemoryStream(bytes);
        await request.Body.FlushAsync();
        request.Body.Seek(0, SeekOrigin.Begin);
    }

    public static async Task<T> TryDeserializeRequestAsync<T>(
        this HttpRequest request) where T : class
    {
        try
        {
            var result = await request.ReadFromJsonAsync<T>();
            return result ?? throw new InvalidRequestBodyEndpointForgeException();
        }
        catch (JsonException exception)
        {
            throw new BadRequestEndpointForgeException([exception.Message]);
        }
        catch
        {
            throw new EmptyRequestBodyEndpointForgeException();
        }
    }
}