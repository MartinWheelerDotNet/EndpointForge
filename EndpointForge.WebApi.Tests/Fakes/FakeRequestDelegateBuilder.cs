using System.Diagnostics.CodeAnalysis;
using EndpointForge.Abstractions.Interfaces;
using EndpointForge.Abstractions.Models;
using Microsoft.AspNetCore.Http;

namespace EndpointForge.WebApi.Tests.Fakes;

[ExcludeFromCodeCoverage]
public class FakeRequestDelegateBuilder: IRequestDelegateBuilder
{
    public bool HasBeenCalled { get; private set; }

    public RequestDelegate BuildResponse(
        EndpointResponseDetails responseDetails,
        List<EndpointForgeParameterDetails> parameters)
    {
        HasBeenCalled = true;
        return async _ => { await Task.CompletedTask; };
    }
}