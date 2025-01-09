using EndpointForge.Abstractions.Models;
using Microsoft.AspNetCore.Http;

namespace EndpointForge.Abstractions.Interfaces;

public interface IRequestDelegateBuilder
{
    RequestDelegate BuildResponse(
        EndpointResponseDetails responseDetails, 
        List<EndpointForgeParameterDetails> parameters);
}