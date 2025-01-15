using EndpointForge.Models;
using EndpointForge.WebApi.Processors;
using FluentAssertions;

namespace EndpointForge.WebApi.Tests.Processors;

public class ParameterProcessorTests
{
    [Fact]
    public void When_ProcessWithStaticParameter_Expect_ParameterMappedIntoDictionary()
    {
        var parameters = new List<EndpointParameterDetails>
        {
            new("static", "test-parameter-name", "parameter-value")
        };
        var expectedParameterDictionary = new Dictionary<string, object>
        {
            { "test-parameter-name", "parameter-value" }
        };
        var parameterProcessor = new ParameterProcessor();
        var httpContext = new DefaultHttpContext();
        var processedParameterDictionary = parameterProcessor.Process(parameters, httpContext);
        
        processedParameterDictionary.Should().BeEquivalentTo(expectedParameterDictionary);
    }
    
    [Fact]
    public void When_ProcessWithHeaderParameter_Expect_ParameterMappedFromHeadersIntoDictionary()
    {
        var parameters = new List<EndpointParameterDetails>
        {
            new("header", "XCustom-Header", "test-parameter-name")
        };
        var expectedParameterDictionary = new Dictionary<string, object>
        {
            { "test-parameter-name", "test-parameter-value" }
        };
        var parameterProcessor = new ParameterProcessor();
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Headers.Append("XCustom-Header", "test-parameter-value");
        
        var processedParameterDictionary = parameterProcessor.Process(parameters, httpContext);
        
        processedParameterDictionary.Should().BeEquivalentTo(expectedParameterDictionary);
    }
    
        
    [Fact]
    public void When_ProcessWithRoutePath_Expect_ParameterMappedFromRoutePathIntoDictionary()
    {
        List<EndpointParameterDetails> parameters = [];
        var expectedParameterDictionary = new Dictionary<string, object>
        {
            { "path-parameter-name", "path-parameter-value" }
        };
        var parameterProcessor = new ParameterProcessor();
        var httpContext = new DefaultHttpContext();
        httpContext.Request.RouteValues.Add("path-parameter-name", "path-parameter-value");
        
        var processedParameterDictionary = parameterProcessor.Process(parameters, httpContext);
        
        processedParameterDictionary.Should().BeEquivalentTo(expectedParameterDictionary);
    }
}