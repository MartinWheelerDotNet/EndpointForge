using EndpointForge.Abstractions.Interfaces;
using EndpointForge.Abstractions.Models;

namespace EndpointForge.WebApi.Processors;

public class ParameterProcessor : IParameterProcessor
{
    public Dictionary<string, string> Process(List<EndpointForgeParameterDetails> parameters, HttpContext context)
    {
        var processedParameters = new Dictionary<string, string>();
        foreach (var parameter in parameters)
        {
            switch (parameter.Type)
            {
                case "static":
                    processedParameters.Add(parameter.Identifier, parameter.Value);
                    break;
                case "header":
                    if (context.Request.Headers.TryGetValue(parameter.Identifier, out var header))
                        processedParameters.Add(parameter.Value, header.ToString());
                    break;
            }
        }

        return processedParameters;
    }
}