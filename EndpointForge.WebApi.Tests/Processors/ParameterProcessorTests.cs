using EndpointForge.Core.Models;
using EndpointForge.WebApi.Processors;
using FluentAssertions;
using Microsoft.AspNetCore.Http;

namespace EndpointForge.WebApi.Tests.Processors;

public class ParameterProcessorTests
{
    [Fact]
    public void When_ProcessWithStaticParameter_Expect_ParameterMappedInDictionary()
    {
        var parameters = new List<EndpointParameterDetails>
        {
            new("static", "identifier", "parameter-value")
        };
        var expectedParameterDictionary = new Dictionary<string, object>
        {
            { "identifier", "parameter-value" }
        };
        var parameterProcessor = new ParameterProcessor();
        var httpContext = new DefaultHttpContext();
        var processedParameterDictionary = parameterProcessor.Process(parameters, httpContext);
        
        processedParameterDictionary.Should().BeEquivalentTo(expectedParameterDictionary);
    }
    
    [Fact]
    public void When_ProcessWithHeaderParameter_Expect_ParameterMappedFromHeadersInToDictionary()
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
}