namespace EndpointForge.WebApi.Exceptions;

public class DataSourceNotRegisteredEndpointForgeException() : Exception(
    """
    EndpointForgeDataSource has not been registered yet.
    Did you forget to call AddEndpointForge() on the service collection?
    """);