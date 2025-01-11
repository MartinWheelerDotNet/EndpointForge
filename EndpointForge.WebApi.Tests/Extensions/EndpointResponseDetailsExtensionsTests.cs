using EndpointForge.Models;
using EndpointForge.WebApi.Extensions;
using EndpointForge.WebApi.Tests.Attributes;
using FluentAssertions;

namespace EndpointForge.WebApi.Tests.Extensions;

public class EndpointResponseDetailsExtensionsTests
{
    [Theory]
    [StringNullEmptyOrWhitespaceInlineData]
    [InlineData("test-body", true)]
    public void When_HasBodyIsCalled(string? body, bool expectedResult)
    {
        var endpointResponseDetails = new EndpointResponseDetails { Body = body };

        var result = endpointResponseDetails.HasBody();

        result.Should().Be(expectedResult);
    }
    
    [Theory]
    [StringNullEmptyOrWhitespaceInlineData]
    [InlineData("text/test-type", true)]
    public void When_HasContentTypeIsCalled(string? contentType, bool expectedResult)
    {
        EndpointResponseDetails endpointResponseDetails = new() { ContentType = contentType };

        var result = endpointResponseDetails.HasContentType();

        result.Should().Be(expectedResult);
    }
}