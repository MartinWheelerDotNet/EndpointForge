using System.Net.Http.Json;
using EndpointForge.IntegrationTests.Fixtures;
using EndpointForge.Abstractions.Models;
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
        var expectedResponseBody = new
        {
            StatusCode = HttpStatusCode.BadRequest,
            Errors = new[]
            {
                "Request body must not be empty."
            }
        };
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
        var addEndpointRequest = new
        {
            Methods = new[]
            {
                "GET"
            }
        };
        var expectedResponseBody = new
        {
            StatusCode = HttpStatusCode.UnprocessableEntity,
            Errors = new[]
            {
                "Request body is invalid."
            }
        };
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
        var addEndpointRequest = new
        {
            Route = "/test-unprocessable-entity"
        };
        var expectedResponseBody = new
        {
            StatusCode = HttpStatusCode.UnprocessableEntity,
            Errors = new[]
            {
                "Request body is invalid."
            }
        };
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
        var addEndpointRequest = new
        {
            Route = string.Empty,
            Methods = Array.Empty<string>()
        };
        var expectedResponseBody = new
        {
            StatusCode = HttpStatusCode.UnprocessableEntity,
            Errors = new[]
            {
                "Endpoint request `route` is empty or whitespace",
                "Endpoint request `methods` contains no entries"
            }
        };
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
            Methods = new[]
            {
                "GET"
            }
        };
        var expectedResponseBody = new
        {
            StatusCode = HttpStatusCode.Conflict,
            Errors = new[]
            {
                "The requested endpoint has already been added for GET method"
            }
        };
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
            Methods = new[]
            {
                "GET"
            }
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
            Methods = new[]
            {
                "GET"
            },
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
            Methods = new[]
            {
                "GET"
            },
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
            Methods = new[]
            {
                "GET"
            }
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
            Methods = new[]
            {
                "GET"
            }
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
            Methods = new[]
            {
                "GET", 
                "POST"
            }
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

    #region Response Body Tests

    [Fact]
    public async Task ProvidingNoBodyProvidesNoContentTypeHeaderAndNoBody()
    {
        var addEndpointRequest = new
        {
            Route = "/test-endpoint-response-no-body",
            Methods = new[]
            {
                "GET"
            },
            Response = new
            {
                StatusCode = 200
            }
        };
        using var httpClient = webApiFixture.Application.CreateHttpClient(WebApiName);

        await httpClient.PostAsJsonAsync(AddEndpointRoute, addEndpointRequest);
        var response = await httpClient.GetAsync(addEndpointRequest.Route);
        var responseBody = await response.Content.ReadAsStringAsync();

        Assert.Multiple(
            () => response.Content.Headers.ContentType.Should().BeNull(),
            () => responseBody.Should().BeEmpty(),
            () => response.Content.Headers.ContentLength.Should().Be(0));
    }

    [Fact]
    public async Task ProvidingBodyWithNoContentTypeProvidesContentTypeHeaderAsPlainTextWithBody()
    {
        var addEndpointRequest = new
        {
            Route = "/test-endpoint-response-body-no-content-type",
            Methods = new[]
            {
                "GET"
            },
            Response = new
            {
                StatusCode = 201,
                Body = "String response body."
            }
        };
        using var httpClient = webApiFixture.Application.CreateHttpClient(WebApiName);
        
        await httpClient.PostAsJsonAsync(AddEndpointRoute, addEndpointRequest);
        var response = await httpClient.GetAsync(addEndpointRequest.Route);
        var responseBody = await response.Content.ReadAsStringAsync();

        Assert.Multiple(
            () => response.Content.Headers.ContentType!.MediaType.Should().Be("text/plain"),
            () => response.Content.Headers.ContentLength.Should().Be(21),
            () => responseBody.Should().Be("String response body."));
    }

    [Fact]
    public async Task ProvidingResponseBodyAndTestTypeContentTypeProvidesContentTypeHeaderAsTestTextWithBody()
    {
        var addEndpointRequest = new
        {
            Route = "/test-endpoint-response-body-with-content-type",
            Methods = new[]
            {
                "GET"
            },
            Response = new
            {
                StatusCode = 201,
                ContentType = "text/test-type",
                Body = "String response body."
            }
        };
        using var httpClient = webApiFixture.Application.CreateHttpClient(WebApiName);

        await httpClient.PostAsJsonAsync(AddEndpointRoute, addEndpointRequest);
        var response = await httpClient.GetAsync(addEndpointRequest.Route);
        var responseBody = await response.Content.ReadAsStringAsync();

        Assert.Multiple(
            () => response.Content.Headers.ContentType!.MediaType.Should().Be("text/test-type"),
            () => response.Content.Headers.ContentLength.Should().Be(21),
            () => responseBody.Should().Be("String response body."));
    }
    #endregion
}