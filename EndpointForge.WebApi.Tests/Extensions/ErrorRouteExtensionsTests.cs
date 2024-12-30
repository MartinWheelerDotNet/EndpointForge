using System.Net;
using EndpointForge.Abstractions.Models;
using EndpointForge.WebApi.Extensions;
using FluentAssertions;
using Microsoft.AspNetCore.Http.HttpResults;

namespace EndpointForge.WebApi.Tests.Extensions;

public class ErrorRouteExtensionsTests
{
    #region GetTypedResult Tests
    [Fact]
    public void When_GetTypedResultWithErrorResponseWithStatusCodeUnprocessableEntity_ReturnsUnprocessableEntity()
    {
        ErrorResponse errorResponse = new(HttpStatusCode.UnprocessableEntity, []);

        var result = errorResponse.GetTypedResult();
        
        result.Should().BeOfType<UnprocessableEntity<ErrorResponse>>();
    }
    
    [Fact]
    public void When_GetTypedResultWithErrorResponseWithStatusCodeConflict_ReturnsConflict()
    {
        ErrorResponse errorResponse = new(HttpStatusCode.Conflict, []);

        var result = errorResponse.GetTypedResult();
        
        result.Should().BeOfType<Conflict<ErrorResponse>>();
    }

    [Theory]
    [InlineData(HttpStatusCode.BadRequest)]
    [InlineData(HttpStatusCode.Ambiguous)]
    [InlineData(HttpStatusCode.Forbidden)]
    public void When_GetTypedResultWithErrorResponseWithAnyOtherStatusCode_ReturnsBadRequest(HttpStatusCode statusCode)
    {
        ErrorResponse errorResponse = new(statusCode, []);
        
        var result = errorResponse.GetTypedResult();
        
        result.Should().BeOfType<BadRequest<ErrorResponse>>();
    }
    #endregion
}