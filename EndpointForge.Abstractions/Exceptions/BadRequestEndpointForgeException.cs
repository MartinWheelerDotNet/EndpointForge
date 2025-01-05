using System.Net;

namespace EndpointForge.Abstractions.Exceptions;

public class BadRequestEndpointForgeException(IEnumerable<string>? errors) : EndpointForgeException(
    HttpStatusCode.BadRequest, 
    "Request body was of an unknown type or is missing required fields.",
    errors);