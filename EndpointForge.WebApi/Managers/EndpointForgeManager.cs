using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using EndpointForge.WebApi.Extensions;
using EndpointForge.Abstractions.Interfaces;
using EndpointForge.Abstractions.Models;

namespace EndpointForge.WebApi.Managers;

public class EndpointForgeManager(
    IEndpointForgeDataSource endpointForge, ILogger<EndpointForgeManager> logger) : IEndpointForgeManager
{
    private const string ConflictMessage = "The requested endpoint has already been added for {0} method";
    private const string RouteMissingOrEmptyMessage = "Endpoint request `route` is empty or whitespace";
    private const string MethodMissingOrEmptyMessage = "Endpoint request `methods` contains no entries";
    private const string EmptyRequestBodyMessage = "Request body must not be empty";
    
    private readonly ConcurrentDictionary<EndpointRoutingDetails, EndpointResponseDetails> _endpointDetails = new();
    private readonly Lock _dictionaryLock = new();
    
    public async Task<IResult> TryAddEndpointAsync(AddEndpointRequest addEndpointRequest)
    {
        if (!TryValidateEndpointRequest(addEndpointRequest, out var validationErrorResponse))
        {
            logger.LogErrorResponse(validationErrorResponse);
            return TypedResults.UnprocessableEntity(validationErrorResponse); 
        }
        
        logger.LogInformation("Successfully validated the add endpoint request.");

        if (!TryCreateEndpoint(addEndpointRequest, out var conflictErrorResponse))
        {
            logger.LogErrorResponse(conflictErrorResponse);
            return conflictErrorResponse.GetTypedResult();
        }
        
        logger.LogAddEndpointRequestCompleted(addEndpointRequest);
        
        return TypedResults.Created(addEndpointRequest.Route, addEndpointRequest);
    }

    private static bool TryValidateEndpointRequest(
        [NotNullWhen(true)] AddEndpointRequest? addEndpointRequest, 
        [NotNullWhen(false)] out ErrorResponse? errorResponse)
    {
        errorResponse = null;
        if (addEndpointRequest is null)
        {
            errorResponse = new ErrorResponse(HttpStatusCode.UnprocessableEntity, [EmptyRequestBodyMessage]);
            return false;
        }

        List<string> errors = [];
        
        if (string.IsNullOrWhiteSpace(addEndpointRequest.Route))
            errors.Add(RouteMissingOrEmptyMessage);
        if (addEndpointRequest.Methods.Count == 0)
            errors.Add(MethodMissingOrEmptyMessage);

        if (errors.Count is 0)
            return true;
        
        errorResponse = new ErrorResponse(HttpStatusCode.UnprocessableEntity, errors);
        return false;
    }

    private bool TryCreateEndpoint(
        AddEndpointRequest addEndpointRequest, 
        [NotNullWhen(false)] out ErrorResponse? errorResponse)
    {
        errorResponse = null;

        /*
            Lock the dictionary to ensure no other threads attempt to add or remove from the dictionary
            between checking if all methods can be added, and adding them.  This is for transactional
            reasons to prevent some methods being added to the dictionary and not others. 
        */
        lock (_dictionaryLock)
        {
            var errors = addEndpointRequest.GetEndpointRoutingDetails()
                .Where(details => _endpointDetails.ContainsKey(details))
                .Select(detail => string.Format(ConflictMessage, detail.Method))
                .ToList();

            if (errors.Count is not 0)
            {
                errorResponse = new ErrorResponse(HttpStatusCode.Conflict, errors);
                return false;
            }

            foreach (var detail in addEndpointRequest.GetEndpointRoutingDetails())
                _endpointDetails.TryAdd(detail, addEndpointRequest.Response);

            endpointForge.AddEndpoint(addEndpointRequest);
            return true; 
        }
    }
}