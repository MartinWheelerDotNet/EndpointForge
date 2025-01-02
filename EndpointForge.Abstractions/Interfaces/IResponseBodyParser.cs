namespace EndpointForge.Abstractions.Interfaces;

public interface IResponseBodyParser
{
    Task ProcessResponseBody(Stream stream, string responseBody, IDictionary<string, string> parameters);
}