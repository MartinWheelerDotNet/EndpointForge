namespace EndpointForge.Abstractions.Exceptions;

public class EndpointForgeDataSourceNotRegisteredException() : Exception(
    """
    EndpointForgeDataSource has not been registered yet.
    Did you forget to call AddEndpointForge() on the service collection?
    """);