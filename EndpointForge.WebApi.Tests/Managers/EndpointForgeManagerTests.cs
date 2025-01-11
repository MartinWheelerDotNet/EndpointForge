using EndpointForge.Abstractions;
using EndpointForge.Models;
using EndpointForge.WebApi.Exceptions;
using EndpointForge.WebApi.Managers;
using EndpointForge.WebApi.Validators;
using FluentAssertions;
using Microsoft.AspNetCore.Http.HttpResults;
using Moq;

namespace EndpointForge.WebApi.Tests.Managers;

public class EndpointForgeManagerTests
{
    private readonly Mock<IEndpointForgeDataSource> _mockEndpointForgeDataSource = new();

    private readonly EndpointForgeManager _endpointForgeManager;

    public EndpointForgeManagerTests()
    {
        var stubLogger = NullLogger<EndpointForgeManager>.Instance;
        var parametersValidator = new EndpointParameterDetailsValidator();
        var validator = new AddEndpointRequestValidator(parametersValidator);
        _endpointForgeManager = new EndpointForgeManager(stubLogger, _mockEndpointForgeDataSource.Object, validator);
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

        Assert.Multiple(
            () => _mockEndpointForgeDataSource.Verify(dataSource => dataSource.AddEndpoint(addEndpointRequest, true)),
            () => result.Should().BeOfType<Created>());
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
        
        var exception = await Assert.ThrowsAsync<InvalidRequestBodyEndpointForgeException>(
            async () => await _endpointForgeManager.TryAddEndpointAsync(addEndpointRequest));
        
        Assert.Multiple(
            () => exception.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity),
            () => exception.Message.Should().Be("Request contains invalid JSON body which cannot be processed."),
            () => exception.Errors
                .Should()
                .BeEquivalentTo("Endpoint request `route` is empty or whitespace."));
    }

    [Fact]
    public async Task When_AddEndpointRequestFailsValidation_Expect_UnprocessableEntityWithValidationErrorMessage()
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
        
        var exception = await Assert.ThrowsAsync<InvalidRequestBodyEndpointForgeException>(
            async () => await _endpointForgeManager.TryAddEndpointAsync(addEndpointRequest));
        
        Assert.Multiple(
            () => exception.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity),
            () => exception.Message.Should().Be("Request contains invalid JSON body which cannot be processed."),
            () => exception.Errors
                .Should()
                .BeEquivalentTo("Endpoint request `methods` contains no entries."));
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
         
        await _endpointForgeManager.TryAddEndpointAsync(addEndpointRequest);

        var exception = await Assert.ThrowsAsync<ConflictEndpointForgeException>(
            async () => await _endpointForgeManager.TryAddEndpointAsync(addEndpointRequest));
        
        Assert.Multiple(
                () => exception.StatusCode.Should().Be(HttpStatusCode.Conflict),
                () => exception.Message.Should().Be("Request contains one or more route conflicts."),
                () => exception.Errors
                    .Should()
                    .BeEquivalentTo("The requested endpoint has already been added for GET method"));
    }
}