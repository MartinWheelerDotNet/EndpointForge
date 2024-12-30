using EndpointForge.Abstractions.Extensions;
using EndpointForge.Abstractions.Models;
using FluentAssertions;

namespace EndpointForge.Abstractions.Tests.Extensions;

public class EndpointResponseDetailsExtensionsTests
{
    [Theory]
    [StringNullEmptyOrWhitespaceInlineData]
    [InlineData("test-body", true)]
    public void When_HasBodyIsCalled(string? body, bool expectedResult)
    {
        EndpointResponseDetails endpointResponseDetails = new() { Body = body };

        var result = endpointResponseDetails.HasBody();

        result.Should().Be(expectedResult);
    }
    
    [Theory]
    [StringNullEmptyOrWhitespaceInlineData]
    [InlineData("text/test-type", true)]
    public void When_HasContentTypeIsCalled(string? body, bool expectedResult)
    {
        EndpointResponseDetails endpointResponseDetails = new() { Body = body };

        var result = endpointResponseDetails.HasBody();

        result.Should().Be(expectedResult);
    }
}