using System.Net;
using EndpointForge.Abstractions.Exceptions;
using EndpointForge.Abstractions.Models;
using EndpointForge.WebApi.Extensions;
using FluentAssertions;
using Microsoft.AspNetCore.Http;

namespace EndpointForge.WebApi.Tests.Extensions;

public class HttpRequestExtensionsTests
{
    #region TryDeserializeRequestAsync Tests

    [Fact]
    public async Task When_DeserializingRequestBodyAndBodyIsValid_Expect_Deserialized()
    {
        AddEndpointRequest addEndpointRequest = new()
        {
            Route = "/test-route",
            Methods = ["GET", "POST", "PUT", "DELETE"]
        };
        var httpContext = new DefaultHttpContext();
        await httpContext.Request.WriteAsJsonAsync(addEndpointRequest);

        var result = await httpContext.Request.TryDeserializeRequestAsync<AddEndpointRequest>();

        Assert.Multiple(
            () => result.Should().BeOfType<AddEndpointRequest>(),
            () => result.Should().BeEquivalentTo(addEndpointRequest));
    }
    
    [Fact]
    public async Task When_DeserializingRequestBodyAndBodyIsIncorrectType_Expect_BadRequestException()
    {
        var incorrectType = new
        {
            UnknownField = "unknown-field-value"
        };
        
        var httpContext = new DefaultHttpContext();
        await httpContext.Request.WriteAsJsonAsync(incorrectType);

        var exception = await Assert.ThrowsAsync<BadRequestEndpointForgeException>(
            async () => await httpContext.Request.TryDeserializeRequestAsync<AddEndpointRequest>());
        
        Assert.Multiple(
            () => exception.StatusCode.Should().Be(HttpStatusCode.BadRequest),
            () => exception.Message.Should().Be("Request body was of an unknown type or is missing required fields."));
    }

    [Fact]
    public async Task When_DeserializingRequestBodyAndBodyIsEmpty_Expect_BadRequestExceptionThrown()
    {
        var httpContext = new DefaultHttpContext();

        await Assert.ThrowsAsync<EmptyRequestBodyEndpointForgeException>(
            async () => await httpContext.Request.TryDeserializeRequestAsync<AddEndpointRequest>());
    }

    #endregion

    #region WriteAsJsonAsync Tests

    [Fact]
    public async Task When_WritingAsJson_Expect_DataWrittenToResponseBody()
    {
        AddEndpointRequest addEndpointRequest = new()
        {
            Route = "/test-route",
            Methods = ["GET", "POST", "PUT", "DELETE"]
        };
        var httpContext = new DefaultHttpContext();

        await httpContext.Request.WriteAsJsonAsync(addEndpointRequest);
        var result = await httpContext.Request.ReadFromJsonAsync<AddEndpointRequest>();

        result.Should().BeEquivalentTo(addEndpointRequest);
    }

    #endregion
}