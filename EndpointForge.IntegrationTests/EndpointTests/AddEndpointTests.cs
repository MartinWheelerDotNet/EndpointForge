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
    public async Task CallingAddEndpointWithoutEndpointDetailsReturnsBadRequest()
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
    public async Task CallingAddEndpointWithEmptyRouteReturnsUnprocessableEntity()
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
    public async Task CallingAddEndpointWithEmptyHttpMethodReturnsUnprocessableEntity()
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
    public async Task CallingAddEndpointWithMultipleMissingElementsReturnsUnprocessableEntityWithMultipleErrors()
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
    public async Task CallingAddEndpointWithDuplicateDetailsReturnsConflict()
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
    public async Task CallingAddEndpointWithWithValidEndpointDetailsReturnsCreated()
    {
        var addEndpointRequest = new { Route = "/test-endpoint-created", Method = "GET", Priority = 0 };
        using var httpClient = webApiFixture.Application.CreateHttpClient(WebApiName);
        
        var response = await httpClient.PostAsJsonAsync(AddEndpointRoute, addEndpointRequest);
        var responseBody = await response.Content.ReadFromJsonAsync<AddEndpointRequest>();

        Assert.Multiple(
            () => response.StatusCode.Should().Be(HttpStatusCode.Created),
            () => response.Headers.Location.Should().Be(addEndpointRequest.Route),
            () => responseBody.Should().BeEquivalentTo(addEndpointRequest));
    }
    
    [Fact]
    public async Task AfterCallingAddEndpointWithNoEndpointResponseEndpointReturnOk()
    {
        var addEndpointRequest = new { Route = "/test-endpoint-available-ok", Method = "GET", Priority = 0 };
        using var httpClient = webApiFixture.Application.CreateHttpClient(WebApiName);
        
        await httpClient.PostAsJsonAsync(AddEndpointRoute, addEndpointRequest);
    
        var response = await httpClient.GetAsync(addEndpointRequest.Route);
        
        Assert.Multiple(
            () => response.StatusCode.Should().Be(HttpStatusCode.OK),
            () => response.Content.Headers.ContentLength.Should().Be(0));
    }
    
    [Fact]
    public async Task AfterCallingAddEndpointResponseEndpointIsOnlyAvailableOnProvidedMethod()
    {
        var addEndpointRequest = new { Route = "/test-endpoint-method-not-found", Method = "GET", Priority = 0 };
        using var httpClient = webApiFixture.Application.CreateHttpClient(WebApiName);
        
        await httpClient.PostAsJsonAsync(AddEndpointRoute, addEndpointRequest);
    
        var response = await httpClient.PostAsync(addEndpointRequest.Route, null);

        response.StatusCode.Should().Be(HttpStatusCode.MethodNotAllowed);
    }
    #endregion
}