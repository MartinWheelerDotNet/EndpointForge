using EndpointForge.Abstractions.Models;

namespace EndpointForge.Abstractions.Interfaces;

public interface IResponseBodyParser
{
    Task ProcessResponseBody(Stream stream, string responseBody, Dictionary<string, string> parameters);
}