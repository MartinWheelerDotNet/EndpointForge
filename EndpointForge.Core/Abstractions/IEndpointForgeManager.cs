using EndpointForge.Core.Models;
using Microsoft.AspNetCore.Http;

namespace EndpointForge.Core.Abstractions;

public interface IEndpointForgeManager
{
    Task<IResult> TryAddEndpointAsync(AddEndpointRequest addEndpointRequest);
}