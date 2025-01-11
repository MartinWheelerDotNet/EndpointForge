namespace EndpointForge.Core.Exceptions;

public class EndpointForgeException(
    HttpStatusCode statusCode,
    string message,
    IEnumerable<string>? errors = null) : Exception(message)
{
    public HttpStatusCode StatusCode { get; } = statusCode;
    public IEnumerable<string>? Errors { get; } = errors;
}
