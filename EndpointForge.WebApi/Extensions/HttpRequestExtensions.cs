using System.Text.Json;
using EndpointForge.Abstractions.Exceptions;

namespace EndpointForge.WebApi.Extensions;

public static class HttpRequestExtensions
{
    private static readonly JsonSerializerOptions Options = new()
    {
        DefaultBufferSize = 128 * 1024,
        PropertyNameCaseInsensitive = true
    };
    
     public static async Task<T> TryDeserializeRequestAsync<T>(this HttpRequest request) where T : class
     {
         try
         {
             await using var stream = new MemoryStream((int) request.Headers.ContentLength.GetValueOrDefault() + 1024);
             await request.Body.CopyToAsync(stream);
             var buffer = stream.GetBuffer();
             var bytesRead = (int) stream.Length;
         
             var jsonSpan = new ReadOnlySpan<byte>(buffer, 0, bytesRead);
             var reader = new Utf8JsonReader(jsonSpan);
    
             var result = JsonSerializer.Deserialize<T>(ref reader, Options);
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