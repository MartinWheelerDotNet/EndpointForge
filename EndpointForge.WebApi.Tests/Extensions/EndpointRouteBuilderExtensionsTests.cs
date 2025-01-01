using EndpointForge.Abstractions.Exceptions;
using EndpointForge.WebApi.DataSources;
using EndpointForge.WebApi.Extensions;
using EndpointForge.WebApi.Tests.Fakes;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.IO;

namespace EndpointForge.WebApi.Tests.Extensions;



public class EndpointRouteBuilderExtensionsTests
{
    [Fact]
    public void When_UseEndpointForgeAndDataSourceIsFound_Expect_DataSourceAddedToDataSources()
    {
        var memoryStreamManager = new RecyclableMemoryStreamManager();
        var stubBuilder = new FakeEndpointRouteBuilder();
        var stubResponseBodyParser = new FakeResponseBodyParser(string.Empty);
        var stubLogger = new NullLogger<EndpointForgeDataSource>();
        var dataSource = new EndpointForgeDataSource(stubLogger, stubResponseBodyParser, memoryStreamManager);
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