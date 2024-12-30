using EndpointForge.Abstractions.Models;
using Microsoft.AspNetCore.Http;

namespace EndpointForge.Abstractions.Interfaces;

public interface IEndpointForgeManager
{
    Task<IResult> TryAddEndpointAsync(AddEndpointRequest addEndpointRequest);
}