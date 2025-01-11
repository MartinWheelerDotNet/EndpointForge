using EndpointForge.Models;

namespace EndpointForge.Abstractions;

public interface IParameterProcessor
{
    Dictionary<string, string> Process(List<EndpointParameterDetails> parameters, HttpContext context);
}