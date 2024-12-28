using System.Text.Json;
using EndpointForge.WebApi.Models;
using EndpointManager.Abstractions.Models;

namespace EndpointForge.WebApi.Deserializers;

public static class RequestDeserializer
{
    private const string InvalidRequestBodyMessage = "Request body is invalid.";
    private const string EmptyRequestBodyMessage = "Request body must not be empty.";

    public static async Task<DeserializeResult<T>> TryDeserializeRequestAsync<T>(HttpRequest request) where T : class
    {
        try
        {
            var addEndpointRequest = await request.ReadFromJsonAsync<T>();
            return new DeserializeResult<T>(addEndpointRequest);
        }
        catch (JsonException)
        {
            return new DeserializeResult<T>
            {
                ErrorResult = TypedResults.UnprocessableEntity(new ErrorResponse([InvalidRequestBodyMessage]))
            };
        }
        catch
        {
            return new DeserializeResult<T>
            {
                ErrorResult = TypedResults.BadRequest(new ErrorResponse([EmptyRequestBodyMessage]))
            };
        }
    }
}
