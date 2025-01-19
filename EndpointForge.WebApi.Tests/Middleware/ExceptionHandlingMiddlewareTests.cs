using EndpointForge.Abstractions.Constants;
using EndpointForge.Models;
using EndpointForge.WebApi.Exceptions;
using EndpointForge.WebApi.Middleware;
using FluentAssertions;

namespace EndpointForge.WebApi.Tests.Middleware;

public class ExceptionHandlingMiddlewareTests
{
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };
    private static readonly ILogger<ExceptionHandlingMiddleware> StubLogger = 
        NullLogger<ExceptionHandlingMiddleware>.Instance;
    
    [Fact]
    public async Task When_EndpointForgeExceptionIsThrown_Expect_ResponseWithCorrectValues()
    {
        const string exceptionMessage = "test-exception-message";
        var errors = new[]
        {
            "test-error-message-1",
            "test-error-message-2"
        };
        
        var endpointForgeException = new EndpointForgeException(
            HttpStatusCode.InternalServerError,
            exceptionMessage, 
            errors);

        var expectedErrorResponse = new ErrorResponse(ErrorStatusCode.InternalServerError, exceptionMessage, errors);

        var middleware = new ExceptionHandlingMiddleware(StubLogger, _ => throw endpointForgeException);
        var context = new DefaultHttpContext
        {
            Response = { Body = new MemoryStream() },
            TraceIdentifier = "test-trace-identifier"
        };
        
        await middleware.Invoke(context);
        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var responseBody = await JsonSerializer.DeserializeAsync<ErrorResponse>(context.Response.Body, JsonOptions);

        Assert.Multiple(
            () => context.Response.StatusCode.Should().Be((int) HttpStatusCode.InternalServerError),
            () => responseBody.Should().BeEquivalentTo(expectedErrorResponse)
        );
    }

    [Fact]
    public async Task When_UnhandledExceptionIsThrown_Expect_ResponseWithInternalServerError()
    {
        const string exceptionMessage = "unhandled-exception-message";
        
        var unhandledException = new Exception(exceptionMessage);
        var expectedErrorResponse = new ErrorResponse(ErrorStatusCode.InternalServerError, exceptionMessage, []);

        var middleware = new ExceptionHandlingMiddleware(StubLogger, _ => throw unhandledException);
        
        var context = new DefaultHttpContext
        {
            Response = { Body = new MemoryStream() },
            TraceIdentifier = "test-trace-identifier"
        };
        
        await middleware.Invoke(context);
        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var responseBody = await JsonSerializer.DeserializeAsync<ErrorResponse>(context.Response.Body, JsonOptions);
        
        Assert.Multiple(
            () => context.Response.StatusCode.Should().Be((int) HttpStatusCode.InternalServerError),
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