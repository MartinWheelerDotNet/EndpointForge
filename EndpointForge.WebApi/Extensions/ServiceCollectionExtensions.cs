using EndpointForge.Abstractions.Generators;
using EndpointForge.WebApi.DataSources;
using EndpointForge.WebApi.Managers;
using EndpointForge.Abstractions.Interfaces;
using EndpointForge.WebApi.Factories;
using EndpointForge.WebApi.Parsers;
using EndpointForge.WebApi.Rules;
using Microsoft.IO;

namespace EndpointForge.WebApi.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddEndpointForge(this IServiceCollection services)
        => services
            .AddCoreServices()
            .AddGenerators()
            .AddRules();

    private static IServiceCollection AddCoreServices(this IServiceCollection services)
        => services
            .AddSingleton<EndpointForgeDataSource>()
            .AddSingleton<RecyclableMemoryStreamManager>()
            .AddSingleton<IEndpointForgeDataSource>(provider => provider.GetRequiredService<EndpointForgeDataSource>())
            .AddSingleton<IEndpointForgeManager, EndpointForgeManager>()
            .AddSingleton<IResponseBodyParser, ResponseBodyParser>()
            .AddSingleton<IEndpointForgeRuleFactory, EndpointForgeRuleFactory>();

    private static IServiceCollection AddGenerators(this IServiceCollection services) 
        => services
            .AddSingleton<IGuidGenerator, GuidGenerator>();
    
    private static IServiceCollection AddRules(this IServiceCollection services)
        => services
            .AddSingleton<IEndpointForgeRule, GenerateGuidRule>()
            .AddSingleton<IEndpointForgeRule, InsertParameterRule>();
}