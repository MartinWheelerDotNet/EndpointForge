using System.Text.Json;
using EndpointForge.Abstractions.Exceptions;

namespace EndpointForge.WebApi.Extensions;

public static class HttpRequestExtensions
{
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