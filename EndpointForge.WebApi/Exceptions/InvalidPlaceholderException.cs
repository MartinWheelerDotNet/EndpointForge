namespace EndpointForge.WebApi.Exceptions;

public class InvalidPlaceholderException(string message, int placeholderPosition) : EndpointForgeException(
    HttpStatusCode.BadRequest, 
    $"An invalid placeholder was found at position [{placeholderPosition}] in the response body",
    [message]);