using System.Text;
using EndpointForge.Models;
using EndpointForge.WebApi.Exceptions;
using EndpointForge.WebApi.Extensions;
using FluentAssertions;

namespace EndpointForge.WebApi.Tests.Extensions;

public class HttpRequestExtensionsTests
{
    #region TryDeserializeRequestAsync Tests

    [Fact]
    public async Task When_DeserializingRequestBodyAndBodyIsValid_Expect_Deserialized()
    {
        var addEndpointRequest = new AddEndpointRequest
        {
            Route = "/test-route",
            Methods = ["GET", "POST", "PUT", "DELETE"],
            Response = {
                StatusCode = 201,
                Body = "test-body"
            }
        };
        
        var httpContext = new DefaultHttpContext();
        await WriteAsJsonAsync(httpContext.Request, addEndpointRequest);
        
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
        await WriteAsJsonAsync(httpContext.Request, incorrectType);

        var exception = await Assert.ThrowsAsync<BadRequestEndpointForgeException>(
            async () => await httpContext.Request.TryDeserializeRequestAsync<AddEndpointRequest>());

        Assert.Multiple(
            () => exception.StatusCode.Should().Be(HttpStatusCode.BadRequest),
            () => exception.Message
                .Should()
                .Be("Request body was of an unknown type, empty, or is missing required fields."));
    }

    [Fact]
    public async Task When_DeserializingRequestBodyAndContentLengthIsZero_Expect_BadRequestException()
    {
        var httpContext = new DefaultHttpContext
        {
            Request = { ContentLength = 0 }
        };

        var exception = await Assert.ThrowsAsync<BadRequestEndpointForgeException>(
            async () => await httpContext.Request.TryDeserializeRequestAsync<AddEndpointRequest>());

        Assert.Multiple(
            () => exception.StatusCode.Should().Be(HttpStatusCode.BadRequest),
            () => exception.Message
                .Should()
                .Be("Request body was of an unknown type, empty, or is missing required fields."),
            () => exception.Errors.Should().BeEquivalentTo("Request body must not be empty."));
    }

    #endregion

    private static async Task WriteAsJsonAsync<T>(HttpRequest request, T data)
    {
        request.ContentType = "application/json";
    
        var json = JsonSerializer.Serialize(data);
        var bytes = Encoding.UTF8.GetBytes(json);

        request.Body = new MemoryStream(bytes);
        await request.Body.FlushAsync();
        request.Body.Seek(0, SeekOrigin.Begin);
        
        request.ContentLength = bytes.Length;

    }
}
