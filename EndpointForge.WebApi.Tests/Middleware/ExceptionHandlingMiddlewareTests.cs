using System.Net;
using System.Text.Json;
using EndpointForge.Abstractions.Exceptions;
using EndpointForge.Abstractions.Models;
using EndpointForge.WebApi.Middleware;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace EndpointForge.WebApi.Tests.Middleware;

public class ExceptionHandlingMiddlewareTests
{
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };
    private static readonly ILogger<ExceptionHandlingMiddleware> StubLogger = 
        NullLogger<ExceptionHandlingMiddleware>.Instance;
    
    [Fact]
    public async Task When_EndpointForgeExceptionIsThrown_Expect_ResponseWithCorrectStatusCodeAndMessage()
    {
        const string exceptionMessage = "test-exception-message";
        var errors = new[]
        {
            "test-error-message-1",
            "test-error-message-2"
        };
        
        var endpointForgeException = new EndpointForgeException(HttpStatusCode.BadRequest, exceptionMessage, errors);
        var expectedErrorResponse = new ErrorResponse(HttpStatusCode.BadRequest, exceptionMessage, errors);

        var middleware = new ExceptionHandlingMiddleware(StubLogger, _ => throw endpointForgeException);
        var context = new DefaultHttpContext
        {
            Response = { Body = new MemoryStream() }
        };
        
        await middleware.Invoke(context);
        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var responseBody = await JsonSerializer.DeserializeAsync<ErrorResponse>(context.Response.Body, JsonOptions);

        Assert.Multiple(
            () => context.Response.StatusCode.Should().Be((int) HttpStatusCode.BadRequest),
            () => responseBody.Should().BeEquivalentTo(expectedErrorResponse)
        );
    }

    [Fact]
    public async Task When_UnhandledExceptionIsThrown_Expect_ResponseWithBadRequestAndGenericMessage()
    {
        const string errorMessage = "test-exception-message";
        const string exceptionMessage = "Request body was of an unknown type or is missing required fields.";
        var unhandledException = new Exception("test-exception-message");
        var expectedErrorResponse = new ErrorResponse(HttpStatusCode.BadRequest, exceptionMessage, [errorMessage]);

        var middleware = new ExceptionHandlingMiddleware(StubLogger, _ => throw unhandledException);
        
        var context = new DefaultHttpContext
        {
            Response = { Body = new MemoryStream() }
        };
        
        await middleware.Invoke(context);
        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var responseBody = await JsonSerializer.DeserializeAsync<ErrorResponse>(context.Response.Body, JsonOptions);
        
        Assert.Multiple(
            () => context.Response.StatusCode.Should().Be((int)HttpStatusCode.BadRequest),
            () => responseBody.Should().BeEquivalentTo(expectedErrorResponse));
    }

    [Fact]
    public async Task When_NoExceptionIsThrown_Expect_RequestToPassThroughMiddleware()
    {
        var middleware = new ExceptionHandlingMiddleware(StubLogger, context =>
        {
            context.Response.StatusCode = (int) HttpStatusCode.OK;
            return Task.CompletedTask;
        });

        var context = new DefaultHttpContext
        {
            Response = { Body = new MemoryStream() }
        };
        
        await middleware.Invoke(context);
        
        Assert.Multiple(
            () => context.Response.StatusCode.Should().Be((int) HttpStatusCode.OK),
            () => context.Response.Body.Length.Should().Be(0)
        );
    }
}