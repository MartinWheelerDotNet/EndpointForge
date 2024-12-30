using System.Net;
using EndpointForge.Abstractions.Models;
using EndpointForge.WebApi.Extensions;
using FluentAssertions;
using Microsoft.AspNetCore.Http;

namespace EndpointForge.WebApi.Tests.Extensions;

public class HttpRequestExtensionsTests
{
    #region TryDeserializeRequestAsync Tests
    [Fact]
    public async Task When_DeserializingRequestBodyAndBodyIsValid_Expect_DeserializeResultWithResult()
    {
        AddEndpointRequest addEndpointRequest = new()
        {
            Route = "/test-route",
            Methods = ["GET", "POST", "PUT", "DELETE"]
        };
        var httpContext = new DefaultHttpContext();
        await httpContext.Request.WriteAsJsonAsync(addEndpointRequest);

        var (result, errors) = await httpContext.Request.TryDeserializeRequestAsync<AddEndpointRequest>();

        Assert.Multiple(
            () => result.Should().BeEquivalentTo(addEndpointRequest),
            () => errors.Should().BeNull());
    }

    [Fact]
    public async Task When_DeserializingRequestBodyAndBodyIsInvalid_Expect_UnprocessableEntityErrorResponse()
    {
        var addEndpointRequest = new
        {
            Route = "/test-route"
        };
        
        var expectedErrorResponse = new ErrorResponse(
            HttpStatusCode.UnprocessableEntity, 
            ["Request body is invalid."]);
        
        var httpContext = new DefaultHttpContext();
        await httpContext.Request.WriteAsJsonAsync(addEndpointRequest);

        var (result, errors) = await httpContext.Request.TryDeserializeRequestAsync<AddEndpointRequest>();

        Assert.Multiple(
            () => result.Should().BeNull(),
            () => errors.Should().BeEquivalentTo(expectedErrorResponse));
    }
    
    [Fact]
    public async Task When_DeserializingRequestBodyAndBodyIsEmpty_Expect_BadRequestErrorResponse()
    {
        var expectedErrorResponse = new ErrorResponse(
            HttpStatusCode.BadRequest,
            ["Request body must not be empty."]);
        
        var httpContext = new DefaultHttpContext();

        var (result, errors) = await httpContext.Request.TryDeserializeRequestAsync<AddEndpointRequest>();

        Assert.Multiple(
            () => result.Should().BeNull(),
            () => errors.Should().BeEquivalentTo(expectedErrorResponse));
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