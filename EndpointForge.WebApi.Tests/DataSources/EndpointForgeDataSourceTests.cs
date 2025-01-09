using System.Collections.Immutable;
using EndpointForge.Abstractions.Interfaces;
using EndpointForge.Abstractions.Models;
using EndpointForge.WebApi.DataSources;
using EndpointForge.WebApi.Tests.Fakes;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.IO;

namespace EndpointForge.WebApi.Tests.DataSources;

public class EndpointForgeDataSourceTests
{
    private readonly ILogger<EndpointForgeDataSource> _stubLogger = new NullLogger<EndpointForgeDataSource>();
    private readonly FakeRequestDelegateBuilder _stubRequestDelegateBuilder = new();

    
    [Fact]
    public void When_AddEndpointWithValidRoute_Expect_EndpointsContainsEndpointWithThatRoute()
    {
        var addEndpointRequest = new AddEndpointRequest
        {
            Route = "/test/route",
            Methods = ["GET"]
        };

        var endpointForgeDataSource = new EndpointForgeDataSource(_stubLogger, _stubRequestDelegateBuilder);
        endpointForgeDataSource.AddEndpoint(addEndpointRequest);

        var endpoint = (RouteEndpoint) endpointForgeDataSource.Endpoints.Single();
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
        
        var endpointForgeDataSource = new EndpointForgeDataSource(_stubLogger, _stubRequestDelegateBuilder);
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
        var mockRequestDelegateBuilder = new FakeRequestDelegateBuilder();
        var endpointForgeDataSource = new EndpointForgeDataSource(_stubLogger, mockRequestDelegateBuilder);
        endpointForgeDataSource.AddEndpoint(addEndpointRequest);
        
        mockRequestDelegateBuilder.HasBeenCalled.Should().BeTrue();
    }
}