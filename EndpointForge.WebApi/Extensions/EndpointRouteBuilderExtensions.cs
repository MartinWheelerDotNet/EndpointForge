using EndpointForge.Abstractions;
using EndpointForge.WebApi.Exceptions;

namespace EndpointForge.WebApi.Extensions;

public static class EndpointRouteBuilderExtensions
{
    public static void UseEndpointForge(this IEndpointRouteBuilder endpoints)
    {
        var dataSource = endpoints.ServiceProvider.GetService<IEndpointForgeDataSource>()
            ?? throw new DataSourceNotRegisteredEndpointForgeException();
        
        endpoints.DataSources.Add((EndpointDataSource) dataSource);
    }

    
}