using EndpointForge.Core.Abstractions;
using EndpointForge.Core.Models;
using EndpointForge.WebApi.Builders;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.IO;
using Moq;

namespace EndpointForge.WebApi.Tests.Builders;

public class RequestDelegateBuilderTests
{
    private readonly RecyclableMemoryStreamManager _recyclableMemoryStreamManager = new();
    private readonly Mock<IResponseBodyParser> _mockResponseBodyParser = new();
    private readonly Mock<IParameterProcessor> _stubParameterProcessor = new();
    private readonly HttpContext _httpContext;
    private readonly RequestDelegateBuilder _requestDelegateBuilder;

    public RequestDelegateBuilderTests()
    {
        _requestDelegateBuilder = new RequestDelegateBuilder(
            NullLogger,
            _mockResponseBodyParser.Object,
            _stubParameterProcessor.Object,
            _recyclableMemoryStreamManager);
        _httpContext = new DefaultHttpContext
        {
            Response = { Body = new MemoryStream() }
        };
    }
    
    private static readonly ILogger<RequestDelegateBuilder> NullLogger = new NullLogger<RequestDelegateBuilder>();
    
    [Fact]
    public async Task When_BuildResponse_Expect_ContextResponseHasStatusCodeSet()
    {
        var endpointResponseDetails = new EndpointResponseDetails
        {
            StatusCode = 201
        };
        
        var responseDelegate = _requestDelegateBuilder.BuildResponse(endpointResponseDetails, []);
        await responseDelegate.Invoke(_httpContext);
        
        _httpContext.Response.StatusCode.Should().Be(201);
    }
    
    [Fact]
    public async Task When_BuildResponseHasNoBody_Expect_ContextResponseHasNoContentTypeSet()
    {
        var endpointResponseDetails = new EndpointResponseDetails
        {
            StatusCode = 201
        };
        
        var responseDelegate = _requestDelegateBuilder.BuildResponse(endpointResponseDetails, []);
        await responseDelegate.Invoke(_httpContext);

        _httpContext.Response.ContentType.Should().BeNull();
    }
    
    [Fact]
    public async Task When_BuildResponseHasBodyAndNoContentType_Expect_ContextResponseIsPlainText()
    {
        var endpointResponseDetails = new EndpointResponseDetails
        {
            StatusCode = 201,
            Body = "test-body"
        };
       
        var responseDelegate = _requestDelegateBuilder.BuildResponse(endpointResponseDetails, []);
        await responseDelegate.Invoke(_httpContext);

        _httpContext.Response.ContentType.Should().Be("text/plain");
    }
    
    [Fact]
    public async Task When_BuildResponseHasBodyAndContentType_Expect_ContextResponseIsSetToContentType()
    {
        var endpointResponseDetails = new EndpointResponseDetails
        {
            StatusCode = 201,
            Body = "test-body",
            ContentType = "application/json"
        };
        var httpContext = new DefaultHttpContext();

        var responseDelegate = _requestDelegateBuilder.BuildResponse(endpointResponseDetails, []);
        await responseDelegate.Invoke(httpContext);

        httpContext.Response.ContentType.Should().Be("application/json");
    }

    [Fact]
    public async Task When_BuildResponseHasBody_Expect_ContextResponseBodyIsTestBody()
    {
        var endpointResponseDetails = new EndpointResponseDetails
        {
            StatusCode = 201,
            Body = "test-body",
            ContentType = "application/json"
        };
        _mockResponseBodyParser
            .Setup(parser => parser.ProcessResponseBody(
                It.IsAny<Stream>(), 
                It.IsAny<string>(), 
                It.IsAny<Dictionary<string, string>>()))
            .Returns(async (Stream stream, string _, Dictionary<string, string> _) =>
            {
                stream.Seek(0, SeekOrigin.Begin);
                await using var writer = new StreamWriter(stream, leaveOpen: true);
                await writer.WriteAsync("test-body");
                await writer.FlushAsync();
            });
        
        var responseDelegate = _requestDelegateBuilder.BuildResponse(endpointResponseDetails, []);
        await responseDelegate.Invoke(_httpContext);

        _httpContext.Response.Body.Seek(0, SeekOrigin.Begin);
        using var reader = new StreamReader(_httpContext.Response.Body);
        var body = await reader.ReadToEndAsync();

        Assert.Multiple(
            () => body.Should().Be("test-body"),
            () => _httpContext.Response.ContentLength.Should().Be(9));
    }
    
    [Fact]
    public async Task When_BuildResponseHasBody_Expect_ParserIsCalledWithProcessedParameters()
    {
        var endpointResponseDetails = new EndpointResponseDetails
        {
            StatusCode = 201,
            Body = "test-body",
            ContentType = "application/json"
        };
        var parameters = new List<EndpointParameterDetails>
        {
            new("insert", "test-parameter", "test-parameter-value")
        };
        var expectedParameterDictionary = new Dictionary<string, string>
        {
            { "test-parameter", "test-parameter-value" }
        };
        _mockResponseBodyParser
            .Setup(parser => parser.ProcessResponseBody(
                It.IsAny<Stream>(), 
                It.IsAny<string>(), 
                expectedParameterDictionary))
            .Returns(async (Stream stream, string _, Dictionary<string, string> _) =>
            {
                stream.Seek(0, SeekOrigin.Begin);
                await using var writer = new StreamWriter(stream, leaveOpen: true);
                await writer.WriteAsync("test-body");
                await writer.FlushAsync();
            });
        _stubParameterProcessor
            .Setup(processor => processor.Process(parameters, It.IsAny<HttpContext>()))
            .Returns(expectedParameterDictionary);
        
        var responseDelegate = _requestDelegateBuilder.BuildResponse(endpointResponseDetails, parameters);
        await responseDelegate.Invoke(_httpContext);
        
        _mockResponseBodyParser.Verify(parser => 
            parser.ProcessResponseBody(It.IsAny<Stream>(), It.IsAny<string>(), expectedParameterDictionary),
            Times.Once);
    }
}
