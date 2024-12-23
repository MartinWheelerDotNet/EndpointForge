using System.Collections.Concurrent;
using System.Text.Json;
using EndpointManager.Abstractions.Interfaces;
using EndpointManager.Abstractions.Models;

namespace EndpointForge.WebApi.Managers;

public class EndpointManager : IEndpointManager
{
    private const string ConflictMessage = "The requested endpoint has already been added.";
    private const string InvalidRequestBodyMessage = "Request body is invalid.";
    private const string EmptyRequestBodyMessage = "Request body must not be empty.";
    private const string UriMissingMessage = "Provided URI is missing.";
    private const string HttpMethodMissingMessage = "HttpMethod is missing.";
    
    private readonly ConcurrentDictionary<AddEndpointRequest, bool> _endpointDetails = new();
    
    public async Task<IResult>  
        TryAddEndpointAsync(HttpRequest httpRequest)
    {
        AddEndpointRequest? addEndpointRequest;

        try
        {
            addEndpointRequest = await httpRequest.ReadFromJsonAsync<AddEndpointRequest>();
        }
        catch (JsonException)
        {
            return TypedResults.UnprocessableEntity(new ErrorResponse(null, null, InvalidRequestBodyMessage));
        }
        catch
        {
            return TypedResults.BadRequest(new ErrorResponse(null, null, EmptyRequestBodyMessage));
        }
        
        if (addEndpointRequest is null)
            return TypedResults.BadRequest(new ErrorResponse(null, null, EmptyRequestBodyMessage));
        if (string.IsNullOrEmpty(addEndpointRequest.Uri))
            return TypedResults.UnprocessableEntity(new ErrorResponse(null, null, UriMissingMessage));
        if (string.IsNullOrEmpty(addEndpointRequest.HttpMethod.Method))
            return TypedResults.UnprocessableEntity(new ErrorResponse(null, null, HttpMethodMissingMessage));
        
        return _endpointDetails.TryAdd(addEndpointRequest, true)
            ? TypedResults.Created(addEndpointRequest.Uri, addEndpointRequest)
            : TypedResults.Conflict(
                new ErrorResponse(addEndpointRequest.Uri, addEndpointRequest.HttpMethod, ConflictMessage));
    }
}