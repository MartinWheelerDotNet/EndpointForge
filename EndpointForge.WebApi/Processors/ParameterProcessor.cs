using EndpointForge.Abstractions;
using EndpointForge.Abstractions.Constants;
using EndpointForge.Models;

namespace EndpointForge.WebApi.Processors;

public class ParameterProcessor : IParameterProcessor
{
    public Dictionary<string, string> Process(List<EndpointParameterDetails> parameters, HttpContext context)
    {
        var processedParameters = new Dictionary<string, string>();
        ProcessQueryParameters(context, processedParameters);
        ProcessRouteValues(context, processedParameters);
        ProcessProvidedParameters(parameters, context, processedParameters);

        return processedParameters;
    }

    private static void ProcessQueryParameters(HttpContext context, Dictionary<string, string> processedParameters)
    {
        foreach (var (key, value) in context.Request.Query)
            if (!string.IsNullOrWhiteSpace(value))
                processedParameters.Add(key, value.ToString());
    }
    
    private static void ProcessRouteValues(HttpContext context, Dictionary<string, string> processedParameters)
    {
        foreach (var (key, value) in context.Request.RouteValues)
            if (value is string stringValue && !string.IsNullOrWhiteSpace(stringValue))
                processedParameters.Add(key, stringValue);
    }

    private static void ProcessProvidedParameters(List<EndpointParameterDetails> parameters, HttpContext context, Dictionary<string, string> processedParameters)
    {
        foreach (var (type, name, value) in parameters)
        { 
            if (ParameterType.IsStatic(type))
                processedParameters.Add(name, value);
            if (ParameterType.IsHeader(type))
                if (context.Request.Headers.TryGetValue(name, out var header))
                    processedParameters.Add(value, header.ToString());
        }
    }
}