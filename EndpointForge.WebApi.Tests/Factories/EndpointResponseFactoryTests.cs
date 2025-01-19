using EndpointForge.Abstractions.Constants;
using EndpointForge.Models;
using EndpointForge.WebApi.Exceptions;
using EndpointForge.WebApi.Factories;
using FluentAssertions;

namespace EndpointForge.WebApi.Tests.Factories;

public class EndpointResponseFactoryTests
{
    [Fact]
    public void When_CreateWithBadRequestEndpointForgeException_Expect_ErrorResponseCreatedWithCorrectValues()
    {
        var exception = new BadRequestEndpointForgeException(["test-error"]);
        var expectedErrorResponse = BuildExpectedErrorResponse(ErrorStatusCode.InvalidRequestBody, exception);
        
        var errorResponse = ErrorResponseFactory.Create(exception);

        errorResponse.Should().BeEquivalentTo(expectedErrorResponse);
    }
    
    [Fact]
    public void When_CreateWithConflictEndpointForgeException_Expect_ErrorResponseCreatedWithCorrectValues()
    {
        var exception = new ConflictEndpointForgeException(["test-error"]);
        var expectedErrorResponse = BuildExpectedErrorResponse(ErrorStatusCode.RouteConflict, exception);
        
        var errorResponse = ErrorResponseFactory.Create(exception);

        errorResponse.Should().BeEquivalentTo(expectedErrorResponse);
    }
    
    [Fact]
    public void When_CreateWithInvalidRequestBodyEndpointForgeException_Expect_ErrorResponseCreatedWithCorrectValues()
    {
        var exception = new InvalidRequestBodyEndpointForgeException(["test-error"]);
        var expectedErrorResponse = BuildExpectedErrorResponse(ErrorStatusCode.RequestBodyInvalidJson, exception);
        
        var errorResponse = ErrorResponseFactory.Create(exception);

        errorResponse.Should().BeEquivalentTo(expectedErrorResponse);
    }
    
    [Fact]
    public void When_CreateWithEndpointForgeException_Expect_ErrorResponseCreatedWithCorrectValues()
    {
        var exception = new EndpointForgeException(HttpStatusCode.InternalServerError, "test-message", ["test-error"]);
        var expectedErrorResponse = BuildExpectedErrorResponse(ErrorStatusCode.InternalServerError, exception);
        
        var errorResponse = ErrorResponseFactory.Create(exception);

        errorResponse.Should().BeEquivalentTo(expectedErrorResponse);
    }
    
    [Fact]
    public void When_CreateWithUnhandledException_Expect_ErrorResponseCreatedWithCorrectValues()
    {
        var exception = new Exception("test-message");
        var expectedErrorResponse = new ErrorResponse(ErrorStatusCode.InternalServerError, "test-message", []);
        
        var errorResponse = ErrorResponseFactory.Create(exception);

        errorResponse.Should().BeEquivalentTo(expectedErrorResponse);
    }

    private static ErrorResponse BuildExpectedErrorResponse(string errorStatusCode, EndpointForgeException exception) =>
        new(errorStatusCode, exception.Message, exception.Errors);
}