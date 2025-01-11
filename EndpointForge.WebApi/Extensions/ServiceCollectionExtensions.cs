using EndpointForge.WebApi.DataSources;
using EndpointForge.WebApi.Managers;
using EndpointForge.Abstractions.Interfaces;
using EndpointForge.Abstractions.Models;
using EndpointForge.WebApi.Builders;
using EndpointForge.WebApi.Factories;
using EndpointForge.WebApi.Generators;
using EndpointForge.WebApi.Parsers;
using EndpointForge.WebApi.Processors;
using EndpointForge.WebApi.Rules;
using EndpointForge.WebApi.Validators;
using FluentValidation;
using Microsoft.IO;

namespace EndpointForge.WebApi.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddEndpointForge(this IServiceCollection services)
        => services
            .AddCoreServices()
            .AddValidators()
            .AddGenerators()
            .AddRules();

    private static IServiceCollection AddCoreServices(this IServiceCollection services)
        => services
            .AddSingleton<RecyclableMemoryStreamManager>()
            .AddSingleton<IEndpointForgeDataSource, EndpointForgeDataSource>()
            .AddSingleton<IEndpointForgeManager, EndpointForgeManager>()
            .AddSingleton<IResponseBodyParser, ResponseBodyParser>()
            .AddSingleton<IRequestDelegateBuilder, RequestDelegateBuilder>()
            .AddSingleton<IParameterProcessor, ParameterProcessor>()
            .AddSingleton<IEndpointForgeRuleFactory, EndpointForgeRuleFactory>();

    private static IServiceCollection AddGenerators(this IServiceCollection services) 
        => services
            .AddSingleton<IGuidGenerator, GuidGenerator>();
    
    private static IServiceCollection AddRules(this IServiceCollection services)
        => services
            .AddSingleton<IEndpointForgeRule, GenerateGuidRule>()
            .AddSingleton<IEndpointForgeRule, InsertParameterRule>();

    private static IServiceCollection AddValidators(this IServiceCollection services)
        => services
            .AddTransient<IValidator<AddEndpointRequest>, AddEndpointRequestValidator>()
            .AddTransient<IValidator<EndpointForgeParameterDetails>, EndpointForgeParameterDetailsValidator>();
}