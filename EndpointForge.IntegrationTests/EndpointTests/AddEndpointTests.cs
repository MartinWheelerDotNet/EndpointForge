using System.Net.Http.Json;
using EndpointForge.IntegrationTests.Fixtures;
using EndpointManager.Abstractions.Models;
using FluentAssertions;

namespace EndpointForge.IntegrationTests.EndpointTests;

public class AddEndpointTests(WebApiFixture webApiFixture) : IClassFixture<WebApiFixture>
{
    private const string WebApiName = "webapi";
    private const string AddEndpointRoute = "/add-endpoint";
    
    #region Error Result Tests
    [Fact]
    public async Task CallingAddEndpointWithoutEndpointDetailsRespondsWithBadRequest()
    {
        ErrorResponse expectedResponseBody = new(["Request body must not be empty."]);
        using var httpClient = webApiFixture.Application.CreateHttpClient(WebApiName);

        var response = await httpClient.PostAsync(AddEndpointRoute, null);
        var responseBody = await response.Content.ReadFromJsonAsync<ErrorResponse>();

        Assert.Multiple(
            () => response.StatusCode.Should().Be(HttpStatusCode.BadRequest),
            () => responseBody.Should().BeEquivalentTo(expectedResponseBody));
    }

    [Fact]
    public async Task CallingAddEndpointWithoutRouteRespondsWithUnprocessableEntity()
    {
        var addEndpointRequest = new { Methods = new[] { "GET" } };
        ErrorResponse expectedResponseBody = new(["Request body is invalid."]);
        using var httpClient = webApiFixture.Application.CreateHttpClient(WebApiName);
        
        var response = await httpClient.PostAsJsonAsync(AddEndpointRoute, addEndpointRequest);
        var responseBody = await response.Content.ReadFromJsonAsync<ErrorResponse>();

        Assert.Multiple(
            () => response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity),
            () => responseBody.Should().BeEquivalentTo(expectedResponseBody));
    }
    
    [Fact]
    public async Task CallingAddEndpointWithoutMethodsRespondsWithUnprocessableEntity()
    {
        var addEndpointRequest = new { Route = "/test-unprocessable-entity" };
        ErrorResponse expectedResponseBody = new(["Request body is invalid."]);
        using var httpClient = webApiFixture.Application.CreateHttpClient(WebApiName);
        
        var response = await httpClient.PostAsJsonAsync(AddEndpointRoute, addEndpointRequest);
        var responseBody = await response.Content.ReadFromJsonAsync<ErrorResponse>();
        
        Assert.Multiple(
            () => response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity),
            () => responseBody.Should().BeEquivalentTo(expectedResponseBody));
    }
    
    [Fact]
    public async Task CallingAddEndpointWithMultipleEmptyElementsRespondsWithUnprocessableEntityWithMultipleErrors()
    {
        var addEndpointRequest = new { Route = string.Empty, Priority = 0, Methods = Array.Empty<string>() };
        ErrorResponse expectedResponseBody = new(
            [
                "Endpoint request `route` is empty or whitespace.",
                "Endpoint request `methods` contains no entries."
            ]);
        using var httpClient = webApiFixture.Application.CreateHttpClient(WebApiName);
        
        var response = await httpClient.PostAsJsonAsync(AddEndpointRoute, addEndpointRequest);
        var responseBody = await response.Content.ReadFromJsonAsync<ErrorResponse>();

        Assert.Multiple(
            () => response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity),
            () => responseBody.Should().BeEquivalentTo(expectedResponseBody));
    }
    #endregion
    
    #region Conflict Result Tests
    [Fact]
    public async Task CallingAddEndpointWithDuplicateDetailsRespondsWithConflict()
    {
        var addEndpointRequest = new
        {
            Route = "/test-endpoint-conflict", 
            Methods = new[] { "GET" }
        };
        ErrorResponse expectedResponseBody = new(["The requested endpoint has already been added for GET method."]);
        
        using var httpClient = webApiFixture.Application.CreateHttpClient(WebApiName);
        await httpClient.PostAsJsonAsync(AddEndpointRoute, addEndpointRequest);
        
        var response = await httpClient.PostAsJsonAsync(AddEndpointRoute, addEndpointRequest);
        var responseBody = await response.Content.ReadFromJsonAsync<ErrorResponse>();
    
        Assert.Multiple(
            () => response.StatusCode.Should().Be(HttpStatusCode.Conflict),
            () => responseBody.Should().BeEquivalentTo(expectedResponseBody));
    }
    #endregion
    
    #region Created Result Tests
    [Fact]
    public async Task CallingAddEndpointWithWithValidEndpointDetailsRespondsWithCreated()
    {
        var addEndpointRequest = new
        {
            Route = "/test-endpoint-created", 
            Methods = new[] { "GET" }, 
            Priority = 0, 
            Response = new { StatusCode = 200 }
        };
        using var httpClient = webApiFixture.Application.CreateHttpClient(WebApiName);
        
        var response = await httpClient.PostAsJsonAsync(AddEndpointRoute, addEndpointRequest);
        var responseBody = await response.Content.ReadFromJsonAsync<AddEndpointRequest>();

        Assert.Multiple(
            () => response.StatusCode.Should().Be(HttpStatusCode.Created),
            () => response.Headers.Location.Should().Be(addEndpointRequest.Route),
            () => responseBody.Should().BeEquivalentTo(addEndpointRequest));
    }
    #endregion
    
    #region Response StatusCode Tests
    
    [Fact]
    public async Task ProvidingResponseStatusCode400RespondsWithBadRequest() 
    {
        var addEndpointRequest = new
        {
            Route = "/test-endpoint-response-status-code-400",
            Methods = new[] { "GET" }, 
            Priority = 0,
            Response = new
            {
                StatusCode = 400
            }
        };
        using var httpClient = webApiFixture.Application.CreateHttpClient(WebApiName);
        
        await httpClient.PostAsJsonAsync(AddEndpointRoute, addEndpointRequest);
    
        var response = await httpClient.GetAsync(addEndpointRequest.Route);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest); 
    }
    
    [Fact]
    public async Task ProvidingResponseStatusCode201RespondsWithCreated() 
    {
        var addEndpointRequest = new
        {
            Route = "/test-endpoint-response-status-code-201",
            Methods = new[] { "GET" }, 
            Priority = 0,
            Response = new
            {
                StatusCode = 201
            }
        };
        using var httpClient = webApiFixture.Application.CreateHttpClient(WebApiName);
        
        await httpClient.PostAsJsonAsync(AddEndpointRoute, addEndpointRequest);
    
        var response = await httpClient.GetAsync(addEndpointRequest.Route);

        response.StatusCode.Should().Be(HttpStatusCode.Created); 
    }
    
    [Fact]
    public async Task ProvidingNoEndpointResponseRespondsWithOk() 
    {
        var addEndpointRequest = new
        {
            Route = "/test-endpoint-no-endpoint-response",
            Methods = new[] { "GET" }, 
            Priority = 0
        };
        using var httpClient = webApiFixture.Application.CreateHttpClient(WebApiName);
        
        await httpClient.PostAsJsonAsync(AddEndpointRoute, addEndpointRequest);
    
        var response = await httpClient.GetAsync(addEndpointRequest.Route);

        response.StatusCode.Should().Be(HttpStatusCode.OK); 
    }
    #endregion
    
    #region Response Method Tests

    [Fact]
    public async Task ProvidingOnlyOneMethodCreatesEndpointOnlyOnProvidedMethod()
    {
        var addEndpointRequest = new
        {
            Route = "/test-endpoint-method-not-found", 
            Methods = new[] { "GET" },
            Priority = 0
        };
        using var httpClient = webApiFixture.Application.CreateHttpClient(WebApiName);
        
        await httpClient.PostAsJsonAsync(AddEndpointRoute, addEndpointRequest);
    
        var response = await httpClient.PostAsync(addEndpointRequest.Route, null);

        response.StatusCode.Should().Be(HttpStatusCode.MethodNotAllowed);
    }
    
    [Fact]
    public async Task ProvidingMultipleMethodsCreatesEndpointsForThoseMethods()
    {
        var addEndpointRequest = new
        {
            Route = "/test-endpoint-multiple-methods",
            Methods = new[] { "GET", "POST" }, 
            Priority = 0
        };
        using var httpClient = webApiFixture.Application.CreateHttpClient(WebApiName);
        
        await httpClient.PostAsJsonAsync(AddEndpointRoute, addEndpointRequest);
    
        var getResponse = await httpClient.GetAsync(addEndpointRequest.Route);
        var postResponse = await httpClient.PostAsync(addEndpointRequest.Route, null);

        Assert.Multiple(
            () => getResponse.StatusCode.Should().Be(HttpStatusCode.OK), 
            () => postResponse.StatusCode.Should().Be(HttpStatusCode.OK));
    }
    
    #endregion
}