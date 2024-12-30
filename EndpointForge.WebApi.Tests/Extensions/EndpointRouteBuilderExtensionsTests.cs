using EndpointForge.Abstractions.Exceptions;
using EndpointForge.WebApi.DataSources;
using EndpointForge.WebApi.Extensions;
using EndpointForge.WebApi.Tests.Fakes;
using FluentAssertions;

namespace EndpointForge.WebApi.Tests.Extensions;

public class EndpointRouteBuilderExtensionsTests
{
    [Fact]
    public void When_UseEndpointForgeAndDataSourceIsFound_Expect_DataSourceAddedToDataSources()
    {
        var stubBuilder = new FakeEndpointRouteBuilder();
        var dataSource = new EndpointForgeDataSource();
        stubBuilder.ServiceProvider = new FakeServiceProvider(dataSource);
        
        stubBuilder.UseEndpointForge();

        stubBuilder.DataSources.Should().Contain(dataSource);
    }

    [Fact]
    public void When_UseEndpointForgeAndDataSourceIsNotFound_Expect_ExceptionThrown()
    {
        var stubBuilder = new FakeEndpointRouteBuilder
        {
            ServiceProvider = new FakeServiceProvider(null)
        };
        
        stubBuilder.Invoking(builder => builder.UseEndpointForge())
            .Should()
            .ThrowExactly<EndpointForgeDataSourceNotRegisteredException>();
    }
}