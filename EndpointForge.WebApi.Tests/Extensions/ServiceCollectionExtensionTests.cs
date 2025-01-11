using EndpointForge.Abstractions;
using EndpointForge.WebApi.DataSources;
using EndpointForge.WebApi.Extensions;
using EndpointForge.WebApi.Generators;
using EndpointForge.WebApi.Managers;
using EndpointForge.WebApi.Parsers;
using EndpointForge.WebApi.Rules;
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
    [InlineData(typeof(IEndpointForgeRule), typeof(GenerateGuidRule))]
    [InlineData(typeof(IEndpointForgeRule), typeof(InsertParameterRule))]
    public void When_AddEndpointForge_RequiredEndpointForgesRulesAreAvailable(Type interfaceType, Type expectedImplementationType)
    {
        var serviceCollection = new ServiceCollection();
        
        serviceCollection.AddEndpointForge();
        var serviceProvider = serviceCollection.AddLogging().BuildServiceProvider();
        
        serviceProvider.GetServices(interfaceType)
            .Should()
            .ContainSingle(service => service != null && service.GetType() == expectedImplementationType);
        
    }
}