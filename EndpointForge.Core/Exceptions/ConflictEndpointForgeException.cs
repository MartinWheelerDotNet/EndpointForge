namespace EndpointForge.Core.Exceptions;

public class ConflictEndpointForgeException(IEnumerable<string>? errors = null) :EndpointForgeException(
    HttpStatusCode.Conflict, 
    "Request contains one or more route conflicts.",
    errors);