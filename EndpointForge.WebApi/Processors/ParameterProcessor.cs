using EndpointForge.Abstractions;
using EndpointForge.Abstractions.Constants;
using EndpointForge.Models;

namespace EndpointForge.WebApi.Processors;

public class ParameterProcessor : IParameterProcessor
{
    public Dictionary<string, string> Process(List<EndpointParameterDetails> parameters, HttpContext context)
    {
        var processedParameters = new Dictionary<string, string>();

        foreach (var (key, value) in context.Request.RouteValues)
            if (value is string stringValue && !string.IsNullOrWhiteSpace(stringValue))
                processedParameters.Add(key, stringValue);

        foreach (var (type, name, value) in parameters)
        {
            if (ParameterType.IsStatic(type))
                processedParameters.Add(name, value);
            if (ParameterType.IsHeader(type))
                if (context.Request.Headers.TryGetValue(name, out var header))
                    processedParameters.Add(value, header.ToString());
        }

        return processedParameters;
    }
}