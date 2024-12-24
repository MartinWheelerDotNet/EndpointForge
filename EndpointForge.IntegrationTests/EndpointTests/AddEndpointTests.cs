using System.Net.Http.Json;
using EndpointManager.Abstractions.Models;

namespace EndpointForge.IntegrationTests.EndpointTests;

public class AddEndpointTests(WebApiTests webApiTests) : IClassFixture<WebApiTests>
{
    private const string WebApiName = "webapi";
    private const string AddEndpointName = "add-endpoint";

    [Fact]
    public async Task CallingAddEndpointWithoutEndpointDetailsReturnsBadRequest()
    {
        ErrorResponse expectedResponseBody = new(null, null, "Request body must not be empty.");
        using var httpClient = webApiTests.Application.CreateHttpClient(WebApiName);

        var response = await httpClient.PostAsync($"/{AddEndpointName}", null);
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
        using var httpClient = webApiTests.Application.CreateHttpClient(WebApiName);
        
        var response = await httpClient.PostAsJsonAsync($"{AddEndpointName}", addEndpointRequest);
        var responseBody = await response.Content.ReadFromJsonAsync<ErrorResponse>();

        Assert.Multiple(
            () => Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode),
            () => Assert.Equal(expectedResponseBody, responseBody));
    }

    [Fact]
    public async Task CallingAddEndpointWithWithValidEndpointDetailsReturnsCreated()
    {
        AddEndpointRequest addEndpointRequest = new("/test-endpoint-created", HttpMethod.Get);
        using var httpClient = webApiTests.Application.CreateHttpClient(WebApiName);
        
        var response = await httpClient.PostAsJsonAsync($"{AddEndpointName}", addEndpointRequest);
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
        
        using var httpClient = webApiTests.Application.CreateHttpClient(WebApiName);
        await httpClient.PostAsJsonAsync($"{AddEndpointName}", addGetEndpointRequest);
        
        var response = await httpClient.PostAsJsonAsync($"{AddEndpointName}", addPostEndpointRequest);
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
        using var httpClient = webApiTests.Application.CreateHttpClient(WebApiName);
        await httpClient.PostAsJsonAsync($"{AddEndpointName}", addEndpointRequest);
        
        var response = await httpClient.PostAsJsonAsync($"{AddEndpointName}", addEndpointRequest);
        var responseBody = await response.Content.ReadFromJsonAsync<ErrorResponse>();

        Assert.Multiple(
            () => Assert.Equal(HttpStatusCode.Conflict, response.StatusCode),
            () => Assert.Equal(expectedResponseBody, responseBody));
    }
}