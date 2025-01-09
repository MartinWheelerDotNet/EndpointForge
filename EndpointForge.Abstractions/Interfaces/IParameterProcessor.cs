using EndpointForge.Abstractions.Models;
using Microsoft.AspNetCore.Http;

namespace EndpointForge.Abstractions.Interfaces;

public interface IParameterProcessor
{
    Dictionary<string, string> Process(List<EndpointForgeParameterDetails> parameters, HttpContext context);
}