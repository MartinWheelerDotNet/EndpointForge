using EndpointForge.Abstractions.Generators;
using EndpointForge.Abstractions.Interfaces;
using EndpointForge.WebApi.DataSources;
using EndpointForge.WebApi.Extensions;
using EndpointForge.WebApi.Managers;
using EndpointForge.WebApi.Parsers;
using EndpointForge.WebApi.Rules;
using EndpointForge.WebApi.Writers;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace EndpointForge.WebApi.Tests.Extensions;

public class ServiceCollectionExtensionsTests
{
    [Theory]
    [InlineData(typeof(IEndpointForgeDataSource), typeof(EndpointForgeDataSource))]
    [InlineData(typeof(IEndpointForgeManager), typeof(EndpointForgeManager))]
    [InlineData(typeof(IResponseBodyParser), typeof(ResponseBodyParser))]
    public void When_AddEndpointForge_RequiredCoreServicesAreAvailable(Type interfaceType, Type expectedImplementationType)
    {
        var serviceCollection = new ServiceCollection();
        
        serviceCollection.AddEndpointForge();
        var serviceProvider = serviceCollection.AddLogging().BuildServiceProvider();
        
        serviceProvider.GetService(interfaceType).Should().BeOfType(expectedImplementationType);
    }
    
    [Theory]
    [InlineData(typeof(IGuidGenerator), typeof(GuidGenerator))]
    public void When_AddEndpointForge_RequiredGeneratorsAreAvailable(Type interfaceType, Type expectedImplementationType)
    {
        var serviceCollection = new ServiceCollection();
        
        serviceCollection.AddEndpointForge();
        var serviceProvider = serviceCollection.AddLogging().BuildServiceProvider();
        
        serviceProvider.GetService(interfaceType).Should().BeOfType(expectedImplementationType);
    }
    
    [Theory]
    [InlineData(typeof(IGuidWriter), typeof(GuidWriter))]
    public void When_AddEndpointForge_RequiredRuleWritersAreAvailable(Type interfaceType, Type expectedImplementationType)
    {
        var serviceCollection = new ServiceCollection();
        
        serviceCollection.AddEndpointForge();
        var serviceProvider = serviceCollection.AddLogging().BuildServiceProvider();
        
        serviceProvider.GetService(interfaceType).Should().BeOfType(expectedImplementationType);
    }
    
    [Theory]
    [InlineData(typeof(IEndpointForgeGeneratorRule), typeof(GenerateGuidRule))]
    public void When_AddEndpointForge_RequiredEndpointForgesRulesAreAvailable(Type interfaceType, Type expectedImplementationType)
    {
        var serviceCollection = new ServiceCollection();
        
        serviceCollection.AddEndpointForge();
        var serviceProvider = serviceCollection.AddLogging().BuildServiceProvider();
        
        serviceProvider.GetService(interfaceType).Should().BeOfType(expectedImplementationType);
    }
}