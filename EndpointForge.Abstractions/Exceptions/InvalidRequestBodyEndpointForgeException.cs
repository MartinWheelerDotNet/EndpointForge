using System.Net;

namespace EndpointForge.Abstractions.Exceptions;

public class InvalidRequestBodyEndpointForgeException(IEnumerable<string>? errors = null) : EndpointForgeException(
    HttpStatusCode.UnprocessableEntity, 
    "Request contains invalid JSON body which cannot be processed.", 
    errors);