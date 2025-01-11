using EndpointForge.Core.Models;
using Microsoft.AspNetCore.Http;

namespace EndpointForge.Core.Abstractions;

public interface IParameterProcessor
{
    Dictionary<string, string> Process(List<EndpointParameterDetails> parameters, HttpContext context);
}