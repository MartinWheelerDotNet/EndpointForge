using EndpointForge.Core.Models;

namespace EndpointForge.Core.Abstractions;

public interface IRequestDelegateBuilder
{
    RequestDelegate BuildResponse(
        EndpointResponseDetails responseDetails, 
        List<EndpointParameterDetails> parameters);
}