using EndpointForge.Abstractions;
using EndpointForge.Models;
using EndpointForge.WebApi.Exceptions;
using EndpointForge.WebApi.Extensions;
using FluentValidation;

namespace EndpointForge.WebApi.Managers;

public class EndpointForgeManager(
    ILogger<EndpointForgeManager> logger,
    IEndpointForgeDataSource endpointForge,
    IValidator<AddEndpointRequest> validator) : IEndpointForgeManager
{
    private const string ConflictMessage = "The requested endpoint has already been added for {0} method";
    private readonly List<EndpointRoutingDetails> _endpointDetails = [];
    private readonly Lock _listLock = new();
    
    public async Task<IResult> TryAddEndpointAsync(AddEndpointRequest addEndpointRequest)
    {
        var validationResult = await validator.ValidateAsync(addEndpointRequest);
        if (!validationResult.IsValid)
            throw new InvalidRequestBodyEndpointForgeException(
                validationResult.Errors.Select(error => error.ErrorMessage));

        CreateEndpointOrThrow(addEndpointRequest);
        
        logger.LogAddEndpointRequestCompleted(addEndpointRequest);
        
        await Task.CompletedTask;
        return TypedResults.Created(addEndpointRequest.Route);
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