using System.Net.Http.Json;
using EndpointManager.Abstractions.Models;

namespace EndpointForge.IntegrationTests.EndpointTests;

public class AddEndpointTests(WebApiTests webApiTests) : IClassFixture<WebApiTests>
{
    private const string ResourceName = "webapi";
    private const string EndpointName = "add-endpoint";

    [Fact]
    public async Task CallingAddEndpointWithoutEndpointDetailsReturnsBadRequest()
    {
        ErrorResponse expectedResponseBody = new(null, null, "Request body must not be empty.");
        using var httpClient = webApiTests.Application.CreateHttpClient(ResourceName);

        var response = await httpClient.PostAsync($"/{EndpointName}", null);
        var responseBody = await response.Content.ReadFromJsonAsync<ErrorResponse>();

        Assert.Multiple(
            () => Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode),
            () => Assert.Equal(expectedResponseBody, responseBody));
    }

    [Fact]
    public async Task CallingAddEndpointWithEmptyUriReturnsUnprocessableEntity()
    {
        AddEndpointRequest addEndpointRequest = new(string.Empty, HttpMethod.Get);
        ErrorResponse expectedResponseBody = new(null, null, "Provided URI is missing.");
        using var httpClient = webApiTests.Application.CreateHttpClient(ResourceName);
        
        var response = await httpClient.PostAsJsonAsync($"{EndpointName}", addEndpointRequest);
        var responseBody = await response.Content.ReadFromJsonAsync<ErrorResponse>();

        Assert.Multiple(
            () => Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode),
            () => Assert.Equal(expectedResponseBody, responseBody));
    }

    [Fact]
    public async Task CallingAddEndpointWithWithValidEndpointDetailsReturnsCreated()
    {
        AddEndpointRequest addEndpointRequest = new("/test-endpoint-created", HttpMethod.Get);
        using var httpClient = webApiTests.Application.CreateHttpClient(ResourceName);
        
        var response = await httpClient.PostAsJsonAsync($"{EndpointName}", addEndpointRequest);
        var responseBody = await response.Content.ReadFromJsonAsync<AddEndpointRequest>();

        Assert.Multiple(
            () => Assert.Equal(HttpStatusCode.Created, response.StatusCode),
            () => Assert.Equal(addEndpointRequest.Uri, response.Headers.Location!.ToString()),
            () => Assert.Equal(addEndpointRequest, responseBody));
    }
    
    [Fact]
    public async Task CallingAddEndpointWithExistingUriButDifferingHttpMethodReturnsCreated()
    {
        AddEndpointRequest addGetEndpointRequest = new("/test-endpoint-conflict", HttpMethod.Get);
        AddEndpointRequest addPostEndpointRequest = new("/test-endpoint-conflict", HttpMethod.Post);
        
        using var httpClient = webApiTests.Application.CreateHttpClient(ResourceName);
        await httpClient.PostAsJsonAsync($"{EndpointName}", addGetEndpointRequest);
        
        var response = await httpClient.PostAsJsonAsync($"{EndpointName}", addPostEndpointRequest);
        var responseBody = await response.Content.ReadFromJsonAsync<AddEndpointRequest>();

        Assert.Multiple(
            () => Assert.Equal(HttpStatusCode.Created, response.StatusCode),
            () => Assert.Equal(addPostEndpointRequest.Uri, response.Headers.Location!.ToString()),
            () => Assert.Equal(addPostEndpointRequest, responseBody));
    }

    [Fact]
    public async Task CallingAddEndpointWithEndpointDetailsAlreadyExistingReturnsConflict()
    {
        AddEndpointRequest addEndpointRequest = new("/test-endpoint-conflict", HttpMethod.Get);
        ErrorResponse expectedResponseBody = new(
            "/test-endpoint-conflict",
            HttpMethod.Get,
            "The requested endpoint has already been added.");
        using var httpClient = webApiTests.Application.CreateHttpClient(ResourceName);
        await httpClient.PostAsJsonAsync($"{EndpointName}", addEndpointRequest);
        
        var response = await httpClient.PostAsJsonAsync($"{EndpointName}", addEndpointRequest);
        var responseBody = await response.Content.ReadFromJsonAsync<ErrorResponse>();

        Assert.Multiple(
            () => Assert.Equal(HttpStatusCode.Conflict, response.StatusCode),
            () => Assert.Equal(expectedResponseBody, responseBody));
    }
}