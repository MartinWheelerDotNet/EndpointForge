using EndpointForge.WebApi.DataSources;
using EndpointForge.Abstractions.Exceptions;

namespace EndpointForge.WebApi.Extensions;

public static class EndpointRouteBuilderExtensions
{
    public static void UseEndpointForge(this IEndpointRouteBuilder endpoints)
    {
        var dataSource = endpoints.ServiceProvider.GetService<EndpointForgeDataSource>()
            ?? throw new EndpointForgeDataSourceNotRegisteredException();
        
        endpoints.DataSources.Add(dataSource);
    }
    
    
}