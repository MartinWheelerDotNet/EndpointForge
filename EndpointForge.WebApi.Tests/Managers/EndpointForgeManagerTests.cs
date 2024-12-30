using System.Net;
using System.Text.Json;
using EndpointForge.Abstractions.Interfaces;
using EndpointForge.Abstractions.Models;
using EndpointForge.WebApi.Managers;
using EndpointForge.WebApi.Tests.Fakes;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;

namespace EndpointForge.WebApi.Tests.Managers;

public class EndpointForgeManagerTests
{
    private readonly FakeEndpointForgeDataSource _mockEndpointForgeDataSource;
    private readonly EndpointForgeManager _endpointForgeManager;

    public EndpointForgeManagerTests()
    {
        var stubLogger = NullLogger<EndpointForgeManager>.Instance;
        _mockEndpointForgeDataSource = new FakeEndpointForgeDataSource();
        _endpointForgeManager = new EndpointForgeManager(_mockEndpointForgeDataSource, stubLogger);
    }

    [Fact]
    public async Task When_AddEndpointRequestIsValid_Expect_EndpointIsCreated()
    {
        var addEndpointRequest = new AddEndpointRequest
        {
            Route = "/test",
            Methods = ["GET"],
            Response = new EndpointResponseDetails
            {
                StatusCode = 200,
                ContentType = "application/json",
                Body = "Body"
            }
        };

        var result = await _endpointForgeManager.TryAddEndpointAsync(addEndpointRequest);

        _mockEndpointForgeDataSource.AddedEndpoints.Should().ContainSingle(e => e == addEndpointRequest);
        result.Should().BeOfType<Created<AddEndpointRequest>>();
    }

    [Fact]
    public async Task When_AddEndpointRequestIsNull_Expect_UnprocessableEntityWithEmptyRequestBodyMessage()
    {
        var httpContext = GetHttpContext();

        var result = await _endpointForgeManager.TryAddEndpointAsync(null!);
        var errorResponse = await DeserializeErrorResponseFromResult(result, httpContext);

        Assert.Multiple(
            () => result.Should().BeOfType<UnprocessableEntity<ErrorResponse>>(),
            () => httpContext.Response.StatusCode.Should().Be(422),
            () => errorResponse.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity),
            () => errorResponse.Errors.Should().BeEquivalentTo("Request body must not be empty"));
    }

    [Fact]
    public async Task When_AddEndpointRequestHasEmptyRoute_Expect_UnprocessableEntityWithRouteEmptyMessage()
    {
        var addEndpointRequest = new AddEndpointRequest
        {
            Route = "",
            Methods = ["GET"],
            Response = new EndpointResponseDetails
            {
                StatusCode = 200
            }
        };
        var httpContext = GetHttpContext();

        var result = await _endpointForgeManager.TryAddEndpointAsync(addEndpointRequest);
        var errorResponse = await DeserializeErrorResponseFromResult(result, httpContext);

        Assert.Multiple(
            () => result.Should().BeOfType<UnprocessableEntity<ErrorResponse>>(),
            () => httpContext.Response.StatusCode.Should().Be(422),
            () => errorResponse.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity),
            () => errorResponse.Errors.Should().BeEquivalentTo("Endpoint request `route` is empty or whitespace"));
    }
    
    [Fact]
    public async Task When_AddEndpointRequestHasEmptyMethod_Expect_UnprocessableEntityWithRouteEmptyMessage()
    {
        var addEndpointRequest = new AddEndpointRequest
        {
            Route = "/test-route",
            Methods = [],
            Response = new EndpointResponseDetails
            {
                StatusCode = 200
            }
        };
        var httpContext = GetHttpContext();

        var result = await _endpointForgeManager.TryAddEndpointAsync(addEndpointRequest);
        var errorResponse = await DeserializeErrorResponseFromResult(result, httpContext);

        Assert.Multiple(
            () => result.Should().BeOfType<UnprocessableEntity<ErrorResponse>>(),
            () => httpContext.Response.StatusCode.Should().Be(422),
            () => errorResponse.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity),
            () => errorResponse.Errors.Should().BeEquivalentTo("Endpoint request `methods` contains no entries"));
    }
    
    [Fact]
    public async Task When_AddEndpointRequestHasEmptyRouteAndEmptyMethod_Expect_UnprocessableEntityWithBothMessages()
    {
        var addEndpointRequest = new AddEndpointRequest
        {
            Route = "",
            Methods = [],
            Response = new EndpointResponseDetails
            {
                StatusCode = 200
            }
        };
        var httpContext = GetHttpContext();

        var result = await _endpointForgeManager.TryAddEndpointAsync(addEndpointRequest);
        var errorResponse = await DeserializeErrorResponseFromResult(result, httpContext);

        Assert.Multiple(
            () => result.Should().BeOfType<UnprocessableEntity<ErrorResponse>>(),
            () => httpContext.Response.StatusCode.Should().Be(422),
            () => errorResponse.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity),
            () => errorResponse.Errors.Should().BeEquivalentTo(
                "Endpoint request `route` is empty or whitespace",
                "Endpoint request `methods` contains no entries"));
    }
    
    [Fact]
    public async Task When_AddEndpointRequestHasConflictingRoutes_Expect_ConflictWithConflictMessage()
    {
        var addEndpointRequest = new AddEndpointRequest
        {
            Route = "/test",
            Methods = ["GET"],
            Response = new EndpointResponseDetails
            {
                StatusCode = 200
            }
        };
        var httpContext = GetHttpContext();
        await _endpointForgeManager.TryAddEndpointAsync(addEndpointRequest);
        
        var result = await _endpointForgeManager.TryAddEndpointAsync(addEndpointRequest);
        var errorResponse = await DeserializeErrorResponseFromResult(result, httpContext);

        Assert.Multiple(
            () => result.Should().BeOfType<Conflict<ErrorResponse>>(),
            () => httpContext.Response.StatusCode.Should().Be(409),
            () => errorResponse.StatusCode.Should().Be(HttpStatusCode.Conflict),
            () => errorResponse.Errors
                .Should()
                .BeEquivalentTo("The requested endpoint has already been added for GET method"));
    }

    private static DefaultHttpContext GetHttpContext() => new()
    {
        RequestServices = new ServiceCollection().AddLogging().BuildServiceProvider(),
        Response = { Body = new MemoryStream() }
    };

    private static async Task<ErrorResponse> DeserializeErrorResponseFromResult(
        IResult result,
        DefaultHttpContext httpContext)
    {
        await result.ExecuteAsync(httpContext);
        httpContext.Response.Body.Seek(0, SeekOrigin.Begin);
        var responseBody = await new StreamReader(httpContext.Response.Body).ReadToEndAsync();
        var jsonSerializerOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var errorResponse = JsonSerializer.Deserialize<ErrorResponse>(responseBody, jsonSerializerOptions);
        return errorResponse!;
    }
}