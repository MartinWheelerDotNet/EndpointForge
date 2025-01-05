using EndpointForge.Abstractions.Exceptions;
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
    
    private readonly List<EndpointRoutingDetails> _endpointDetails = [];
    private readonly Lock _listLock = new();
    
    public async Task<IResult> TryAddEndpointAsync(AddEndpointRequest addEndpointRequest)
    {
        ValidateEndpointRequestOrThrow(addEndpointRequest);

        CreateEndpointOrThrow(addEndpointRequest);
        
        logger.LogAddEndpointRequestCompleted(addEndpointRequest);
        
        await Task.CompletedTask;
        return TypedResults.Created(addEndpointRequest.Route, addEndpointRequest);
    }

    private void ValidateEndpointRequestOrThrow(AddEndpointRequest addEndpointRequest) 
    {
        List<string> errors = [];
        
        if (string.IsNullOrWhiteSpace(addEndpointRequest.Route))
            errors.Add(RouteMissingOrEmptyMessage);
        if (addEndpointRequest.Methods.Count == 0)
            errors.Add(MethodMissingOrEmptyMessage);

        if (errors.Count is not 0) throw new InvalidRequestBodyEndpointForgeException(errors);

        logger.LogInformation("Successfully validated the add endpoint request.");
    }

    private void CreateEndpointOrThrow(AddEndpointRequest addEndpointRequest)
    {
       var errors = new List<string>();

        lock (_listLock)
        {
            foreach (var (route, method) in addEndpointRequest.GetEndpointRoutingDetails())
            {
                var currentErrors = _endpointDetails
                    .Where(details => details.Route == route && details.Method == method)
                    .Select(detail => string.Format(ConflictMessage, detail.Method));
                
                errors.AddRange(currentErrors);
            }

            
            if (errors.Count is not 0) throw new ConflictEndpointForgeException(errors);
            
            _endpointDetails.AddRange(addEndpointRequest.GetEndpointRoutingDetails());

            endpointForge.AddEndpoint(addEndpointRequest);
            
            logger.LogInformation("Successfully validated the add endpoint request.");
        }
    }
}