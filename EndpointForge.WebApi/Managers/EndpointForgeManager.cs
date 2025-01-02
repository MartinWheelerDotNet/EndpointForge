using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using EndpointForge.WebApi.Extensions;
using EndpointForge.Abstractions.Interfaces;
using EndpointForge.Abstractions.Models;

namespace EndpointForge.WebApi.Managers;

public class EndpointForgeManager(
    ILogger<EndpointForgeManager> logger,
    IEndpointForgeDataSource endpointForge) : IEndpointForgeManager
{
    private const string ConflictMessage = "The requested endpoint has already been added for {0} method";
    private const string RouteMissingOrEmptyMessage = "Endpoint request `route` is empty or whitespace";
    private const string MethodMissingOrEmptyMessage = "Endpoint request `methods` contains no entries";
    private const string EmptyRequestBodyMessage = "Request body must not be empty";
    
    private readonly List<EndpointRoutingDetails> _endpointDetails = [];
    private readonly Lock _listLock = new();
    
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
        var conflictErrors = new List<string>();

        lock (_listLock)
        {
            foreach (var (route, method) in addEndpointRequest.GetEndpointRoutingDetails())
            {
                var errors = _endpointDetails
                    .Where(details => details.Route == route && details.Method == method)
                    .Select(detail => string.Format(ConflictMessage, detail.Method));
                
                conflictErrors.AddRange(errors);
            }
            
            if (conflictErrors.Count is not 0)
            {
                errorResponse = new ErrorResponse(HttpStatusCode.Conflict, conflictErrors);
                return false;
            }

            _endpointDetails.AddRange(addEndpointRequest.GetEndpointRoutingDetails());

            endpointForge.AddEndpoint(addEndpointRequest);
        }
        return true; 
    }
}