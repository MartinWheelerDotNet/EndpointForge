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
    public async Task CallingAddEndpointWithEmptyRouteRespondsWithUnprocessableEntity()
    {
        var addEndpointRequest = new { Method = "GET" };
        ErrorResponse expectedResponseBody = new(["Endpoint Request `route` is missing or empty."]);
        using var httpClient = webApiFixture.Application.CreateHttpClient(WebApiName);
        
        var response = await httpClient.PostAsJsonAsync(AddEndpointRoute, addEndpointRequest);
        var responseBody = await response.Content.ReadFromJsonAsync<ErrorResponse>();

        Assert.Multiple(
            () => response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity),
            () => responseBody.Should().BeEquivalentTo(expectedResponseBody));
    }
    
    [Fact]
    public async Task CallingAddEndpointWithEmptyHttpMethodRespondsWithUnprocessableEntity()
    {
        var addEndpointRequest = new { Route = "/test-unprocessable-entity" };
        ErrorResponse expectedResponseBody = new(["Endpoint request `method` is missing or empty."]);
        using var httpClient = webApiFixture.Application.CreateHttpClient(WebApiName);
        
        var response = await httpClient.PostAsJsonAsync(AddEndpointRoute, addEndpointRequest);
        var responseBody = await response.Content.ReadFromJsonAsync<ErrorResponse>();

        Assert.Multiple(
            () => response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity),
            () => responseBody.Should().BeEquivalentTo(expectedResponseBody));
    }
    
    [Fact]
    public async Task CallingAddEndpointWithMultipleMissingElementsRespondsWithUnprocessableEntityWithMultipleErrors()
    {
        var addEndpointRequest = new { Priority = 0 };
        ErrorResponse expectedResponseBody = new(
            [
                "Endpoint Request `route` is missing or empty.",
                "Endpoint request `method` is missing or empty."
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
        var addEndpointRequest = new { Route = "/test-endpoint-conflict", Method = "GET" };
        ErrorResponse expectedResponseBody = new(["The requested endpoint has already been added."]);
        
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
        var addEndpointRequest = new { Route = "/test-endpoint-created", Method = "GET", Priority = 0, Response = new { StatusCode = 200 } };
        using var httpClient = webApiFixture.Application.CreateHttpClient(WebApiName);
        
        var response = await httpClient.PostAsJsonAsync(AddEndpointRoute, addEndpointRequest);
        var responseBody = await response.Content.ReadFromJsonAsync<AddEndpointRequest>();

        Assert.Multiple(
            () => response.StatusCode.Should().Be(HttpStatusCode.Created),
            () => response.Headers.Location.Should().Be(addEndpointRequest.Route),
            () => responseBody.Should().BeEquivalentTo(addEndpointRequest));
    }
    
    [Fact]
    public async Task ProvidingOnlyOneMethodCreatesEndpointOnlyOnProvidedMethod()
    {
        var addEndpointRequest = new { Route = "/test-endpoint-method-not-found", Method = "GET", Priority = 0 };
        using var httpClient = webApiFixture.Application.CreateHttpClient(WebApiName);
        
        await httpClient.PostAsJsonAsync(AddEndpointRoute, addEndpointRequest);
    
        var response = await httpClient.PostAsync(addEndpointRequest.Route, null);

        response.StatusCode.Should().Be(HttpStatusCode.MethodNotAllowed);
    }
    #endregion
    
    #region Response StatusCode Tests
    [Fact]
    public async Task ProvidingResponseStatusCode400RespondsWithBadRequest() 
    {
        var addEndpointRequest = new
        {
            Route = "/test-endpoint-response-status-code-400",
            Method = "GET", 
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
            Method = "GET", 
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
            Route = "/test-endpoint-no-response",
            Method = "GET", 
            Priority = 0
        };
        using var httpClient = webApiFixture.Application.CreateHttpClient(WebApiName);
        
        await httpClient.PostAsJsonAsync(AddEndpointRoute, addEndpointRequest);
    
        var response = await httpClient.GetAsync(addEndpointRequest.Route);

        response.StatusCode.Should().Be(HttpStatusCode.OK); 
    }
    #endregion
    
    // #region Priority Tests
    // [Fact]
    // public async Task CallingAddEndpointTwiceWithDifferentPrioritiesRespondsWithHighestPriorityEndpoint()
    // {
    //     const string route = "/test-endpoint-created-priority";
    //     var lowerPriorityRequest = new
    //     {
    //         Route = route,
    //         Method = "GET",
    //         Priority = 0,
    //         Response = new { StatusCode = 200 }
    //     };
    //     var higherPriorityRequest = lowerPriorityRequest with
    //     {
    //         Priority = -1,
    //         Response = new { StatusCode = 400 }
    //     };
    //     
    //     using var httpClient = webApiFixture.Application.CreateHttpClient(WebApiName);
    //     
    //     await httpClient.PostAsJsonAsync(AddEndpointRoute, lowerPriorityRequest);
    //     await httpClient.PostAsJsonAsync(AddEndpointRoute, higherPriorityRequest);
    //
    //     var response = await httpClient.GetAsync(route);
    //
    //     response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    // }
    // #endregion
}