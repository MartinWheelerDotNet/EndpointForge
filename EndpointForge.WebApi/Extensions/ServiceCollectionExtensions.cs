using EndpointForge.WebApi.Managers;
using EndpointForge.WebApi.Models;
using EndpointManager.Abstractions.Interfaces;

namespace EndpointForge.WebApi.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddEndpointForge(this IServiceCollection services)
    {
        services.AddSingleton<EndpointForgeDataSource>();
        services.AddSingleton<IEndpointForgeDataSource>(
            provider => provider.GetRequiredService<EndpointForgeDataSource>());
        services.AddSingleton<IEndpointForgeManager, EndpointForgeManager>();
    }
}