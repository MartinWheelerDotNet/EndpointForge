using System.Net;

namespace EndpointForge.Abstractions.Exceptions;

public class EmptyRequestBodyEndpointForgeException() : EndpointForgeException(
    HttpStatusCode.BadRequest, 
    "Request body is empty or of an unsupported type.");