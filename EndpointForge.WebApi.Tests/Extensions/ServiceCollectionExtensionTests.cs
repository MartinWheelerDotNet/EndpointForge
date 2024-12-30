using EndpointForge.Abstractions.Interfaces;
using EndpointForge.WebApi.DataSources;
using EndpointForge.WebApi.Extensions;
using EndpointForge.WebApi.Managers;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace EndpointForge.WebApi.Tests.Extensions;

public class ServiceCollectionExtensionsTests
{
    [Fact]
    public void When_AddEndpointForge_IEndpointForgeDataSourceIsRegisteredAsEndpointForgeDataSource()
    {
        var serviceCollection = new ServiceCollection();

        serviceCollection.AddEndpointForge();
        var serviceProvider = serviceCollection.BuildServiceProvider();
        var dataSource = serviceProvider.GetService<IEndpointForgeDataSource>();
        
        dataSource
            .Should().NotBeNull()
            .And.BeOfType<EndpointForgeDataSource>();
    }
    
    [Fact]
    public void When_AddEndpointForge_IEndpointForgeManagerIsRegisteredAsEndpointForgeManager()
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddLogging();

        serviceCollection.AddEndpointForge();
        var serviceProvider = serviceCollection.BuildServiceProvider();
        var dataSource = serviceProvider.GetRequiredService<IEndpointForgeManager>();

        dataSource
            .Should().NotBeNull()
            .And.BeOfType<EndpointForgeManager>();
    }
}