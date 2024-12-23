using EndpointManager.Abstractions.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;

namespace EndpointManager.Abstractions.Interfaces;

public interface IEndpointManager
{
    Task<Results<
            Created<AddEndpointRequest>,
            UnprocessableEntity<ErrorResponse>,
            Conflict<ErrorResponse>,
            BadRequest<ErrorResponse>>>
        TryAddEndpointAsync(HttpRequest request);
}