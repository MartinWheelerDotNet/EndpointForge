using System.Collections.Immutable;
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
    private readonly RecyclableMemoryStreamManager _memoryStreamManager = new();
    private readonly ILogger<EndpointForgeDataSource> _stubLogger = new NullLogger<EndpointForgeDataSource>();

    [Fact]
    public void When_AddEndpoint__AddEndpointWithCorrectRoute()
    {
        var addEndpointRequest = new AddEndpointRequest
        {
            Route = "/test/route",
            Methods = ["GET"]
        };
        var stubResponseBodyParser = new FakeResponseBodyParser(string.Empty);

        var endpointForgeDataSource = new EndpointForgeDataSource(
            _stubLogger,
            stubResponseBodyParser,
            _memoryStreamManager);
        endpointForgeDataSource.AddEndpoint(addEndpointRequest);

        var endpoint = (RouteEndpoint) endpointForgeDataSource.Endpoints.Single();
        endpoint.RoutePattern.RawText.Should().Be(addEndpointRequest.Route);
    }

    [Fact]
    public void When_AddEndpointWithParameters_AddEndpointWithCorrectRoute()
    {
        var addEndpointRequest = new AddEndpointRequest
        {
            Route = "/test/route",
            Methods = ["GET"],
            Parameters = [new EndpointForgeParameterDetails("static","test-parameter", "test-parameter-value")]
        };
        var stubResponseBodyParser = new FakeResponseBodyParser(string.Empty);

        var endpointForgeDataSource = new EndpointForgeDataSource(
            _stubLogger,
            stubResponseBodyParser,
            _memoryStreamManager);
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
        var stubResponseBodyParser = new FakeResponseBodyParser(string.Empty);

        var endpointForgeDataSource = new EndpointForgeDataSource(
            _stubLogger,
            stubResponseBodyParser,
            _memoryStreamManager);
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
    public async Task When_AddEndpointWithBodyAndContentType_Expect_EndpointResponseContentTypeIsTestTextAndHasBody()
    {
        var httpContext = new DefaultHttpContext
        {
            Response = { Body = new MemoryStream() }
        };

        var addEndpointRequest = new AddEndpointRequest
        {
            Route = "/test/route",
            Methods = ["GET", "POST"],
            Response = new EndpointResponseDetails
            {
                StatusCode = 200,
                ContentType = "text/test-text",
                Body = "Body"
            }
        };
        var stubResponseBodyParser = new FakeResponseBodyParser("Body");

        var endpointForgeDataSource = new EndpointForgeDataSource(
            _stubLogger,
            stubResponseBodyParser,
            _memoryStreamManager);
        endpointForgeDataSource.AddEndpoint(addEndpointRequest);

        var responseBody = await ExtractResponseBody(endpointForgeDataSource, httpContext);
        Assert.Multiple(
            () => httpContext.Response.StatusCode.Should().Be(200),
            () => httpContext.Response.ContentType.Should().Be("text/test-text"),
            () => httpContext.Response.ContentLength.Should().Be(4),
            () => responseBody.Should().Be("Body"));
    }

    [Fact]
    public async Task When_AddEndpointWithBodyAndNoContentType_Expect_EndpointResponseContentTypeIsTextPlainAndBody()
    {
        var httpContext = new DefaultHttpContext
        {
            Response = { Body = new MemoryStream() }
        };

        var addEndpointRequest = new AddEndpointRequest
        {
            Route = "/test/route",
            Methods = ["GET"],
            Response = new EndpointResponseDetails
            {
                StatusCode = 200,
                Body = "Body"
            }
        };
        var stubResponseBodyParser = new FakeResponseBodyParser("Body");

        var endpointForgeDataSource = new EndpointForgeDataSource(
            _stubLogger,
            stubResponseBodyParser,
            _memoryStreamManager);
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
            Methods = ["GET"],
            Response = new EndpointResponseDetails
            {
                StatusCode = 200,
                ContentType = "text/plain"
            }
        };
        var stubResponseBodyParser = new FakeResponseBodyParser(string.Empty);

        var endpointForgeDataSource = new EndpointForgeDataSource(
            _stubLogger,
            stubResponseBodyParser,
            _memoryStreamManager);

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