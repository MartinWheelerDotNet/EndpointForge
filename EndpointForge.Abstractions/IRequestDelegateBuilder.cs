using EndpointForge.Models;

namespace EndpointForge.Abstractions;

public interface IRequestDelegateBuilder
{
    RequestDelegate BuildResponse(
        EndpointResponseDetails responseDetails, 
        List<EndpointParameterDetails> parameters);
}