using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using EndpointForge.IntegrationTests.Fixtures;
using EndpointForge.Models;
using FluentAssertions;

namespace EndpointForge.IntegrationTests.EndpointTests;

public class AddEndpointTests(EndpointForgeFixture endpointForgeFixture) : IClassFixture<EndpointForgeFixture>
{
    private const string EndpointForgeName = "EndpointForge";
    private const string AddEndpointRoute = "/add-endpoint";

    #region Error Result Tests
    [Fact]
    public async Task When_CallingAddEndpointWithoutEndpointDetails_Expect_BadRequest()
    {
        var expectedResponseBody = new
        {
            ErrorStatusCode = "INVALID_REQUEST_BODY",
            Message = "Request body was of an unknown type, empty, or is missing required fields.",
            Errors = new [] { 
                "Request body must not be empty."
            }
        };
        
        using var httpClient = endpointForgeFixture.Application.CreateHttpClient(EndpointForgeName);

        var response = await httpClient.PostAsync(AddEndpointRoute, null);
        var responseBody = await response.Content.ReadFromJsonAsync<ErrorResponse>();

        Assert.Multiple(
            () => response.StatusCode.Should().Be(HttpStatusCode.BadRequest),
            () => response.Headers.Should().ContainKey("X-Trace-Id"),
            () => responseBody.Should().BeEquivalentTo(expectedResponseBody));
    }

    [Fact]
    public async Task When_CallingAddEndpointWithoutRoute_Expect_UnprocessableEntity()
    {
        var addEndpointRequest = new
        {
            Methods = new[] { "GET" }
        };
        
        var expectedResponseBody = new
        {
            ErrorStatusCode = "INVALID_REQUEST_BODY",
            Message = "Request body was of an unknown type, empty, or is missing required fields.",
            Errors = new [] 
            { 
                "JSON deserialization for type 'EndpointForge.Models.AddEndpointRequest' " +
                "was missing required properties including: 'Route'."
            }
        };
        
        using var httpClient = endpointForgeFixture.Application.CreateHttpClient(EndpointForgeName);
        var jsonBody = JsonSerializer.Serialize(addEndpointRequest);
        var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

        var response = await httpClient.PostAsync(AddEndpointRoute, content);
        var responseBody = await response.Content.ReadFromJsonAsync<ErrorResponse>();

        Assert.Multiple(
            () => response.StatusCode.Should().Be(HttpStatusCode.BadRequest),
            () => response.Headers.Should().ContainKey("X-Trace-Id"),
            () => responseBody.Should().BeEquivalentTo(expectedResponseBody));
    }

    [Fact]
    public async Task When_CallingAddEndpointWithoutMethods_Expect_BadRequest()
    {
        var addEndpointRequest = new
        {
            Route = "/test-unprocessable-entity"
        };
        
        var expectedResponseBody = new
        {
            ErrorStatusCode = "INVALID_REQUEST_BODY",
            Message = "Request body was of an unknown type, empty, or is missing required fields.",
            Errors = new [] 
            { 
                "JSON deserialization for type 'EndpointForge.Models.AddEndpointRequest' " +
                "was missing required properties including: 'Methods'."
            }
        };
        
        using var httpClient = endpointForgeFixture.Application.CreateHttpClient(EndpointForgeName);
        var jsonBody = JsonSerializer.Serialize(addEndpointRequest);
        var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

        var response = await httpClient.PostAsync(AddEndpointRoute, content);
        var responseBody = await response.Content.ReadFromJsonAsync<ErrorResponse>();

        Assert.Multiple(
            () => response.StatusCode.Should().Be(HttpStatusCode.BadRequest),
            () => response.Headers.Should().ContainKey("X-Trace-Id"),
            () => responseBody.Should().BeEquivalentTo(expectedResponseBody));
    }
    
    [Fact]
    public async Task When_CallingAddEndpointWithSingleEmptyElement_Expect_UnprocessableEntityWithSingleError()
    {
        var addEndpointRequest = new
        {
            Route = "/single-empty-element",
            Methods = Array.Empty<string>()
        };
        var expectedResponseBody = new
        {
            ErrorStatusCode = "REQUEST_BODY_INVALID_JSON",
            Message = "Request contains invalid JSON body which cannot be processed.",
            Errors = new [] 
            { 
                $"Endpoint request `{nameof(AddEndpointRequest.Methods)}` contains no entries."
            }
        };
        
        using var httpClient = endpointForgeFixture.Application.CreateHttpClient(EndpointForgeName);
        var jsonBody = JsonSerializer.Serialize(addEndpointRequest);
        var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

        var response = await httpClient.PostAsync(AddEndpointRoute, content);
        var responseBody = await response.Content.ReadFromJsonAsync<ErrorResponse>();

        Assert.Multiple(
            () => response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity),
            () => response.Headers.Should().ContainKey("X-Trace-Id"),
            () => responseBody.Should().BeEquivalentTo(expectedResponseBody));
    }

    [Fact]
    public async Task When_CallingAddEndpointWithMultipleEmptyElements_Expect_UnprocessableEntityWithMultipleErrors()
    {
        var addEndpointRequest = new
        {
            Route = string.Empty,
            Methods = Array.Empty<string>()
        };
        var expectedResponseBody = new
        {
            ErrorStatusCode = "REQUEST_BODY_INVALID_JSON",
            Message = "Request contains invalid JSON body which cannot be processed.",
            Errors = new [] 
            { 
                $"Endpoint request `{nameof(AddEndpointRequest.Route)}` is empty or whitespace.",
                $"Endpoint request `{nameof(AddEndpointRequest.Methods)}` contains no entries."
            }
        };
       
        using var httpClient = endpointForgeFixture.Application.CreateHttpClient(EndpointForgeName);
        var jsonBody = JsonSerializer.Serialize(addEndpointRequest);
        var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

        var response = await httpClient.PostAsync(AddEndpointRoute, content);
        var responseBody = await response.Content.ReadFromJsonAsync<ErrorResponse>();

        Assert.Multiple(
            () => response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity),
            () => response.Headers.Should().ContainKey("X-Trace-Id"),
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
        var expectedResponseBody = new
        {
            ErrorStatusCode = "ROUTE_CONFLICT",
            Message = "Request contains one or more route conflicts.",
            Errors = new[]
            {
                "The requested endpoint has already been added for GET method"
            }
        };
        using var httpClient = endpointForgeFixture.Application.CreateHttpClient(EndpointForgeName);
        var jsonBody = JsonSerializer.Serialize(addEndpointRequest);
        var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

        await httpClient.PostAsync(AddEndpointRoute, content);

        var response = await httpClient.PostAsync(AddEndpointRoute, content);
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
        using var httpClient = endpointForgeFixture.Application.CreateHttpClient(EndpointForgeName);
        var jsonBody = JsonSerializer.Serialize(addEndpointRequest);
        var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

        var response = await httpClient.PostAsync(AddEndpointRoute, content);
        
        Assert.Multiple(
            () => response.StatusCode.Should().Be(HttpStatusCode.Created),
            () => response.Headers.Location.Should().Be(addEndpointRequest.Route));
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
        using var httpClient = endpointForgeFixture.Application.CreateHttpClient(EndpointForgeName);
        var jsonBody = JsonSerializer.Serialize(addEndpointRequest);
        var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

        await httpClient.PostAsync(AddEndpointRoute, content);
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
        using var httpClient = endpointForgeFixture.Application.CreateHttpClient(EndpointForgeName);
        var jsonBody = JsonSerializer.Serialize(addEndpointRequest);
        var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

        await httpClient.PostAsync(AddEndpointRoute, content);
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
        using var httpClient = endpointForgeFixture.Application.CreateHttpClient(EndpointForgeName);
        var jsonBody = JsonSerializer.Serialize(addEndpointRequest);
        var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

        await httpClient.PostAsync(AddEndpointRoute, content);
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
        using var httpClient = endpointForgeFixture.Application.CreateHttpClient(EndpointForgeName);
        var jsonBody = JsonSerializer.Serialize(addEndpointRequest);
        var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

        await httpClient.PostAsync(AddEndpointRoute, content);
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
        using var httpClient = endpointForgeFixture.Application.CreateHttpClient(EndpointForgeName);
        var jsonBody = JsonSerializer.Serialize(addEndpointRequest);
        var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

        await httpClient.PostAsync(AddEndpointRoute, content);
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
        using var httpClient = endpointForgeFixture.Application.CreateHttpClient(EndpointForgeName);
        var jsonBody = JsonSerializer.Serialize(addEndpointRequest);
        var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

        await httpClient.PostAsync(AddEndpointRoute, content);
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
        using var httpClient = endpointForgeFixture.Application.CreateHttpClient(EndpointForgeName);
        var jsonBody = JsonSerializer.Serialize(addEndpointRequest);
        var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

        await httpClient.PostAsync(AddEndpointRoute, content);
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
        using var httpClient = endpointForgeFixture.Application.CreateHttpClient(EndpointForgeName);
        var jsonBody = JsonSerializer.Serialize(addEndpointRequest);
        var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

        await httpClient.PostAsync(AddEndpointRoute, content);
        var response = await httpClient.GetAsync(addEndpointRequest.Route);
        var responseBody = await response.Content.ReadAsStringAsync();

        Assert.Multiple(
            () => response.Content.Headers.ContentType!.MediaType.Should().Be("text/test-type"),
            () => response.Content.Headers.ContentLength.Should().Be(21),
            () => responseBody.Should().Be("String response body."));
    }
    #endregion
    
    #region Parameter Tests

    [Fact]
    public async Task When_ProvidingStaticParameter_Expect_ResponseBodyToContainThatParameter()
    {
        var addEndpointRequest = new
        {
            Route = "/test-endpoint-with-static-parameter",
            Methods = new[] { "GET" },
            Response = new
            {
                StatusCode = 200,
                ContentType = "application/json",
                Body = "{{insert:parameter:test-parameter}}"
            },
            Parameters = new[]
            {
                new
                {
                    Type = "static",
                    Name = "test-parameter",
                    Value = "test-value"
                    
                }
            }
        };
        using var httpClient = endpointForgeFixture.Application.CreateHttpClient(EndpointForgeName);

        await httpClient.PostAsJsonAsync(AddEndpointRoute, addEndpointRequest);
        var jsonBody = JsonSerializer.Serialize(addEndpointRequest);
        var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

        await httpClient.PostAsync(AddEndpointRoute, content);
        var response = await httpClient.GetAsync(addEndpointRequest.Route);
        var responseBody = await response.Content.ReadAsStringAsync();
        
         responseBody.Should().Be("test-value");
    }
    
    [Fact]
    public async Task When_ProvidingHeaderParameter_Expect_EndpointResponseBodyToContainThatHeaderValue()
    {
        const string headerValue = "test-header-value";
        var addEndpointRequest = new
        {
            Route = "/test-endpoint-with-header-value",
            Methods = new[] { "GET" },
            Response = new
            {
                StatusCode = 201,
                ContentType = "text/test-type",
                Body = "{{insert:parameter:test-header-parameter}}"
            },
            Parameters = new[]
            {
                new
                {
                    Type = "header",
                    Name = "XCustom-Header",
                    Value = "test-header-parameter"
                }
            }
        };
        using var httpClient = endpointForgeFixture.Application.CreateHttpClient(EndpointForgeName);
        httpClient.DefaultRequestHeaders.Add("XCustom-Header", headerValue);
        var jsonBody = JsonSerializer.Serialize(addEndpointRequest);
        var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

        await httpClient.PostAsync(AddEndpointRoute, content);
        var response = await httpClient.GetAsync(addEndpointRequest.Route);
        var responseBody = await response.Content.ReadAsStringAsync();
        
        responseBody.Should().Be("test-header-value");
    }
    
    [Fact]
    public async Task When_CapturingGeneratorRuleWithInsertPlaceholder_Expect_EndpointResponseBodyToContainThatValue()
    {
        var addEndpointRequest = new
        {
            Route = "/test-endpoint-with-generator-capture",
            Methods = new[] { "GET" },
            Response = new
            {
                StatusCode = 201,
                ContentType = "text/plain",
                Body = "{{generate:guid:capture-parameter}}:{{insert:parameter:capture-parameter}}"
            }
        };
        using var httpClient = endpointForgeFixture.Application.CreateHttpClient(EndpointForgeName);
        var jsonBody = JsonSerializer.Serialize(addEndpointRequest);
        var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

        await httpClient.PostAsync(AddEndpointRoute, content);
        var response = await httpClient.GetAsync(addEndpointRequest.Route);
        var responseBody = await response.Content.ReadAsStringAsync();
        var capturedValues = responseBody.Split(':');

        capturedValues[0].Should().Be(capturedValues[1]);
    }
    #endregion
    
    #region Route Tests
    [Fact]
    public async Task When_ProvidingPathParameter_Expect_ResponseBodyToContainThatParameter()
    {
        var addEndpointRequest = new
        {
            Route = "/test-endpoint-with/{path-parameter}",
            Methods = new[] { "GET" },
            Response = new
            {
                StatusCode = 200,
                ContentType = "application/json",
                Body = "{{insert:parameter:path-parameter}}"
            }
        };
        
        using var httpClient = endpointForgeFixture.Application.CreateHttpClient(EndpointForgeName);

        await httpClient.PostAsJsonAsync(AddEndpointRoute, addEndpointRequest);
        var jsonBody = JsonSerializer.Serialize(addEndpointRequest);
        var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

        await httpClient.PostAsync(AddEndpointRoute, content);
        var response = await httpClient.GetAsync("/test-endpoint-with/path-parameter-value");
        var responseBody = await response.Content.ReadAsStringAsync();
        
        responseBody.Should().Be("path-parameter-value");
    }
    
    [Fact]
    public async Task When_ProvidingQueryParameters_Expect_ResponseBodyToContainThoseParameters()
    {
        var addEndpointRequest = new
        {
            Route = "/test-query-parameter-endpoint",
            Methods = new[] { "GET" },
            Response = new
            {
                StatusCode = 200,
                ContentType = "application/json",
                Body = "{{insert:parameter:test-id}} {{insert:parameter:test-name}}"
            }
        };
        
        using var httpClient = endpointForgeFixture.Application.CreateHttpClient(EndpointForgeName);

        await httpClient.PostAsJsonAsync(AddEndpointRoute, addEndpointRequest);
        var jsonBody = JsonSerializer.Serialize(addEndpointRequest);
        var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

        await httpClient.PostAsync(AddEndpointRoute, content);
        var response = await httpClient.GetAsync(
            "/test-query-parameter-endpoint?test-id=test-id-value&test-name=test-name-value");
        var responseBody = await response.Content.ReadAsStringAsync();
        
        responseBody.Should().Be("test-id-value test-name-value");
    }
    #endregion

    #region Repeater Tests

    [Fact]
    public async Task When_ProvidingRepeatRule_Expect_EnclosedSegmentIsRepeatedTheSpecifiedNumberOfTimes()
    {
        var addEndpointRequest = new
        {
            Route = "/test-repeat-count-endpoint",
            Methods = new[] { "GET" },
            Response = new
            {
                StatusCode = 200,
                ContentType = "application/json",
                Body = """
                       Not repeated.
                       {{repeat:start:repeat-name:3}}I'm being repeated. {{repeat:end:repeat-name}}
                       Not repeated.
                       """
            }
        };
        
        using var httpClient = endpointForgeFixture.Application.CreateHttpClient(EndpointForgeName);

        await httpClient.PostAsJsonAsync(AddEndpointRoute, addEndpointRequest);
        var jsonBody = JsonSerializer.Serialize(addEndpointRequest);
        var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

        await httpClient.PostAsync(AddEndpointRoute, content);
        var response = await httpClient.GetAsync("/test-repeat-count-endpoint");
        var responseBody = await response.Content.ReadAsStringAsync();

        responseBody.Should().Be("""
                                 Not repeated.
                                 I'm being repeated. I'm being repeated. I'm being repeated. 
                                 Not repeated.
                                 """);
    }

    #endregion
}