using System.Net;
using System.Text;
using System.Text.Json;
using EndpointForge.WebApi.Models;
using EndpointForge.Abstractions.Models;

namespace EndpointForge.WebApi.Extensions;

public static class HttpRequestExtensions
{
    private const string InvalidRequestBodyMessage = "Request body is invalid.";
    private const string EmptyRequestBodyMessage = "Request body must not be empty.";
    
    public static async Task WriteAsJsonAsync<T>(this HttpRequest request, T data)
    {
        request.ContentType = "application/json";
        
        var json = JsonSerializer.Serialize(data);
        var bytes = Encoding.UTF8.GetBytes(json);

        request.Body = new MemoryStream(bytes);
        await request.Body.FlushAsync();
        request.Body.Seek(0, SeekOrigin.Begin);
    }

    public static async Task<DeserializeResult<T>> TryDeserializeRequestAsync<T>(
        this HttpRequest request) where T : class
    {
        try
        {
            var deserializedResult = await request.ReadFromJsonAsync<T>();
            return new DeserializeResult<T>(Result: deserializedResult);
        }
        catch (JsonException)
        {
            return new DeserializeResult<T>(
                ErrorResponse: new ErrorResponse(HttpStatusCode.UnprocessableEntity, [InvalidRequestBodyMessage]));
        }
        catch
        {
            return new DeserializeResult<T>(
                ErrorResponse: new ErrorResponse(HttpStatusCode.BadRequest, [EmptyRequestBodyMessage]));
        }
    }
}