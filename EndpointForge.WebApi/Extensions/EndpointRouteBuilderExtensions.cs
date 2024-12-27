using EndpointForge.WebApi.Models;

namespace EndpointForge.WebApi.Extensions;

public static class EndpointRouteBuilderExtensions
{
    public static void UseEndpointForge(this IEndpointRouteBuilder endpoints)
    {
        var dataSource = endpoints.ServiceProvider.GetService<EndpointForgeDataSource>()
            ?? throw new Exception($"{nameof(EndpointForgeDataSource)} not found.");

        endpoints.DataSources.Add(dataSource);
    }
}