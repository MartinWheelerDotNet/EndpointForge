using EndpointForge.Abstractions.Models;
using EndpointForge.WebApi.DataSources;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace EndpointForge.WebApi.Tests.DataSources;

public class EndpointForgeDataSourceTests
{
    [Fact]
    public void When_AddEndpoint__AddEndpointWithCorrectRoute()
    {
        var addEndpointRequest = new AddEndpointRequest
        {
            Route = "/test/route",
            Methods = [ "GET" ]
        };
        
        var endpointForgeDataSource = new EndpointForgeDataSource();
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
            Methods = [ "GET", "POST" ]
        };
        
        var endpointForgeDataSource = new EndpointForgeDataSource();
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
    public async Task When_AddEndpointWithBodyAndContentType_Expect_EndpointResponseContentTypeIsTestTextAndBody()
    {
        var httpContext = new DefaultHttpContext
        {
            Response = { Body = new MemoryStream() }
        };

        var addEndpointRequest = new AddEndpointRequest
        {
            Route = "/test/route",
            Methods = [ "GET", "POST" ],
            Response = new EndpointResponseDetails
            {
                StatusCode = 200,
                ContentType = "text/test-text",
                Body = "Body"
            }
        };
        var endpointForgeDataSource = new EndpointForgeDataSource();
        
        endpointForgeDataSource.AddEndpoint(addEndpointRequest);
        var responseBody = await ExtractResponseBody(endpointForgeDataSource, httpContext);
        
        Assert.Multiple(
            () => httpContext.Response.StatusCode.Should().Be(200),
            () => httpContext.Response.ContentType.Should().Be("text/test-text"),
            () => httpContext.Response.ContentLength.Should().Be(4), 
            () => responseBody.Should().Be("Body"));
    }
    
    [Fact]
    public async Task When_AddEndpointWithBodyAndNoContentType_Expect_EndpointResponseContentTypeIsTextPlainAndNoBody()
    {
        var httpContext = new DefaultHttpContext
        {
            Response = { Body = new MemoryStream() }
        };

        var addEndpointRequest = new AddEndpointRequest
        {
            Route = "/test/route",
            Methods = [ "GET" ],
            Response = new EndpointResponseDetails
            {
                StatusCode = 200,
                Body = "Body"
            }
        };
        var endpointForgeDataSource = new EndpointForgeDataSource();
        
        endpointForgeDataSource.AddEndpoint(addEndpointRequest);
        var responseBody = await ExtractResponseBody(endpointForgeDataSource, httpContext);
        
        Assert.Multiple(
            () => httpContext.Response.StatusCode.Should().Be(200),
            () => httpContext.Response.ContentType.Should().Be("text/plain"),
            () => httpContext.Response.ContentLength.Should().Be(4), 
            () => responseBody.Should().Be("Body"));
    }
    
    [Fact]
    public async Task When_AddEndpointWithNoBody_Expect_EndpointResponseContentTypeIsEmptyAndNoBody()
    {
        var httpContext = new DefaultHttpContext
        {
            Response = { Body = new MemoryStream() }
        };

        var addEndpointRequest = new AddEndpointRequest
        {
            Route = "/test/route",
            Methods = [ "GET" ],
            Response = new EndpointResponseDetails
            {
                StatusCode = 200,
                ContentType = "text/plain"
            }
        };
        var endpointForgeDataSource = new EndpointForgeDataSource();
        
        endpointForgeDataSource.AddEndpoint(addEndpointRequest);
        var responseBody = await ExtractResponseBody(endpointForgeDataSource, httpContext);

        Assert.Multiple(
            () => httpContext.Response.StatusCode.Should().Be(200),
            () => httpContext.Response.ContentType.Should().BeNull(),
            () => httpContext.Response.ContentLength.Should().BeNull(),
            () => responseBody.Should().BeEmpty());
    }
    
    private static async Task<string> ExtractResponseBody(
        EndpointForgeDataSource endpointForgeDataSource,
        DefaultHttpContext httpContext)
    {
        var requestDelegate = endpointForgeDataSource.Endpoints.Single().RequestDelegate!;
        await requestDelegate(httpContext);
        
        httpContext.Response.Body.Seek(0, SeekOrigin.Begin);
        return await new StreamReader(httpContext.Response.Body).ReadToEndAsync();
    }

}