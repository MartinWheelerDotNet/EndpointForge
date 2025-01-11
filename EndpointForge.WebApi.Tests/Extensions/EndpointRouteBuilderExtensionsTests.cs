using EndpointForge.Abstractions;
using EndpointForge.WebApi.DataSources;
using EndpointForge.WebApi.Exceptions;
using EndpointForge.WebApi.Extensions;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace EndpointForge.WebApi.Tests.Extensions;

public class EndpointRouteBuilderExtensionsTests
{
    private static readonly ILogger<EndpointForgeDataSource> StubLogger = new NullLogger<EndpointForgeDataSource>();
    private static readonly Mock<IEndpointRouteBuilder> MockEndpointRouteBuilder = new();
    
    [Fact]
    public void UseEndpointForge_AddsDataSource_WhenServiceIsRegistered()
    {
        var mockMutableEndpointDataSource = new Mock<MutableEndpointDataSource>(StubLogger)
            .As<IEndpointForgeDataSource>();
        var mockDataSources = new Mock<ICollection<EndpointDataSource>>();
        var mockServiceProvider = new Mock<IServiceProvider>();
        mockServiceProvider
            .Setup(sp => sp.GetService(typeof(IEndpointForgeDataSource)))
            .Returns(mockMutableEndpointDataSource.Object);
        MockEndpointRouteBuilder.SetupGet(er => er.ServiceProvider)
            .Returns(mockServiceProvider.Object);
        MockEndpointRouteBuilder.SetupGet(er => er.DataSources)
            .Returns(mockDataSources.Object);

        MockEndpointRouteBuilder.Object.UseEndpointForge();

        Assert.Multiple(
                () => mockServiceProvider.Verify(
                    serviceProvider => serviceProvider.GetService(typeof(IEndpointForgeDataSource)),
                    Times.Once),
                () => mockDataSources.Verify(
                    dataSources => dataSources.Add((EndpointDataSource) mockMutableEndpointDataSource.Object),
                    Times.Once));
    }

    [Fact]
    public void UseEndpointForge_ThrowsException_WhenServiceIsNotRegistered()
    {
        var mockDataSources = new Mock<ICollection<EndpointDataSource>>();
        var mockServiceProvider = new Mock<IServiceProvider>();
        mockServiceProvider
            .Setup(serviceProvider => serviceProvider.GetService(typeof(IEndpointForgeDataSource)))
            .Returns<IEndpointForgeDataSource>(null!);
        MockEndpointRouteBuilder.SetupGet(er => er.ServiceProvider)
            .Returns(mockServiceProvider.Object);
        MockEndpointRouteBuilder.SetupGet(er => er.DataSources)
            .Returns(mockDataSources.Object);

        Assert.Throws<DataSourceNotRegisteredEndpointForgeException>(
            () => MockEndpointRouteBuilder.Object.UseEndpointForge());

        mockServiceProvider.Verify(sp => sp.GetService(typeof(IEndpointForgeDataSource)), Times.Once);
        mockDataSources.Verify(ds => ds.Add(It.IsAny<EndpointDataSource>()), Times.Never);
    }
}