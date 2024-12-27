using System.Collections.Concurrent;
using System.Text.Json;
using EndpointForge.WebApi.Models;
using EndpointManager.Abstractions.Interfaces;
using EndpointManager.Abstractions.Models;

namespace EndpointForge.WebApi.Managers;

public class EndpointForgeManager(IEndpointForgeDataSource endpointForge) : IEndpointForgeManager
{
    private const string ConflictMessage = "The requested endpoint has already been added.";
    private const string InvalidRequestBodyMessage = "Request body is invalid.";
    private const string EmptyRequestBodyMessage = "Request body must not be empty.";
    private const string RouteMissingOrEmptyMessage = "Endpoint Request `route` is missing or empty.";
    private const string MethodMissingOrEmptyMessage = "Endpoint request `method` is missing or empty.";
    
    private readonly ConcurrentDictionary<AddEndpointRequest, bool> _endpointDetails = new();
    
    public async Task<IResult> TryAddEndpointAsync(HttpRequest httpRequest)
    {
        AddEndpointRequest? addEndpointRequest;

        try
        {
            addEndpointRequest = await httpRequest.ReadFromJsonAsync<AddEndpointRequest>();
        }
        catch(JsonException)
        {
            return TypedResults.UnprocessableEntity(new ErrorResponse([InvalidRequestBodyMessage]));
        }
        catch
        {
            return TypedResults.BadRequest(new ErrorResponse([EmptyRequestBodyMessage]));
        }

        if (addEndpointRequest is null)
            return TypedResults.BadRequest(new ErrorResponse([EmptyRequestBodyMessage]));

        if (!TryValidateAddEndpointRequest(addEndpointRequest, out var errors))
            return TypedResults.UnprocessableEntity(new ErrorResponse(errors));

        if (!_endpointDetails.TryAdd(addEndpointRequest, true))
            return TypedResults.Conflict(new ErrorResponse([ConflictMessage]));
           
        endpointForge.AddEndpoint(addEndpointRequest);
        
        return TypedResults.Created(addEndpointRequest.Route, addEndpointRequest);
    }

    private static bool TryValidateAddEndpointRequest(AddEndpointRequest addEndpointRequest, out List<string> errors)
    {
        errors = [];
        
        if (string.IsNullOrWhiteSpace(addEndpointRequest.Route))
            errors.Add(RouteMissingOrEmptyMessage);
        if (string.IsNullOrWhiteSpace(addEndpointRequest.Method))
            errors.Add(MethodMissingOrEmptyMessage);
        
        return errors.Count is 0;
    }
}