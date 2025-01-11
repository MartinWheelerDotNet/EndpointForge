namespace EndpointForge.Abstractions;

public interface IResponseBodyParser
{
    Task ProcessResponseBody(Stream stream, string responseBody, Dictionary<string, string> parameters);
}