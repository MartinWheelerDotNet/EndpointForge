using EndpointForge.Abstractions;
using EndpointForge.Models;
using EndpointForge.WebApi.DataSources;
using FluentAssertions;
using Moq;

namespace EndpointForge.WebApi.Tests.DataSources;

public class EndpointForgeDataSourceTests
{
    private readonly ILogger<EndpointForgeDataSource> _stubLogger = new NullLogger<EndpointForgeDataSource>();
    private readonly Mock<IRequestDelegateBuilder> _stubRequestDelegateBuilder = new();

    public EndpointForgeDataSourceTests()
    {
        _stubRequestDelegateBuilder
            .Setup(builder => builder.BuildResponse(
                It.IsAny<EndpointResponseDetails>(), 
                It.IsAny<List<EndpointParameterDetails>>()))
            .Returns(async _ => { await Task.CompletedTask; });
    }
    
    [Fact]
    public void When_AddEndpointWithValidRoute_Expect_EndpointsContainsEndpointWithThatRoute()
    {
        var addEndpointRequest = new AddEndpointRequest
        {
            Route = "/test/route",
            Methods = ["GET"]
        };

        var endpointForgeDataSource = new EndpointForgeDataSource(_stubLogger, _stubRequestDelegateBuilder.Object);
        endpointForgeDataSource.AddEndpoint(addEndpointRequest);

        var endpoint = (RouteEndpoint)endpointForgeDataSource.Endpoints.Single();
        endpoint.RoutePattern.RawText.Should().Be(addEndpointRequest.Route);
    }

    [Fact]
    public void When_AddEndpoint_Expect_EndpointAddedWithCorrectMethods()
    {
        var addEndpointRequest = new AddEndpointRequest
        {
            Route = "/test/route",
            Methods = ["GET", "POST"]
        };
        
        var endpointForgeDataSource = new EndpointForgeDataSource(_stubLogger, _stubRequestDelegateBuilder.Object);
        endpointForgeDataSource.AddEndpoint(addEndpointRequest);
    
        var methodMetadata = endpointForgeDataSource
            .Endpoints.Single()
            .Metadata.GetMetadata<HttpMethodMetadata>();
    
        methodMetadata?.HttpMethods
            .Should().NotBeNull()
            .And
            .BeEquivalentTo(addEndpointRequest.Methods);
    }
    
    [Fact]
    public void When_AddEndpoint_Expect_ResponseBodyParserCalled()
    {
        var addEndpointRequest = new AddEndpointRequest
        {
            Route = "/test/route",
            Methods = ["GET", "POST"],
            Response = new EndpointResponseDetails
            {
                StatusCode = 0,
                ContentType = "application/json",
                Body = "Body"
            }
        };
        var endpointForgeDataSource = new EndpointForgeDataSource(_stubLogger, _stubRequestDelegateBuilder.Object);
        endpointForgeDataSource.AddEndpoint(addEndpointRequest);
        
        _stubRequestDelegateBuilder.Verify(builder => builder.BuildResponse(
            addEndpointRequest.Response, 
            new List<EndpointParameterDetails>()));
    }
}