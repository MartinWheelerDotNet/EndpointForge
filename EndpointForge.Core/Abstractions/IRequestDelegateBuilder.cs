using EndpointForge.Core.Models;
using Microsoft.AspNetCore.Http;

namespace EndpointForge.Core.Abstractions;

public interface IRequestDelegateBuilder
{
    RequestDelegate BuildResponse(
        EndpointResponseDetails responseDetails, 
        List<EndpointParameterDetails> parameters);
}