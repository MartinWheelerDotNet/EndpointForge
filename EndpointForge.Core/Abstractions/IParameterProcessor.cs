using EndpointForge.Core.Models;

namespace EndpointForge.Core.Abstractions;

public interface IParameterProcessor
{
    Dictionary<string, string> Process(List<EndpointParameterDetails> parameters, HttpContext context);
}