using EndpointForge.Models;
using EndpointForge.WebApi.Extensions;
using FluentAssertions;

namespace EndpointForge.WebApi.Tests.Extensions;

public class AddEndpointRequestExtensions
{
    [Fact]
    public void When_GetEndpointRequestDetailsAndSingleMethodIsPresent_Expect_TheRouteAndTheMethodAreReturned()
    {
        AddEndpointRequest addEndpointRequest = new()
        {
            Route = "/test-route",
            Methods = ["GET"]
        };

        var endpointRoutingDetails = addEndpointRequest.GetEndpointRoutingDetails();

        endpointRoutingDetails.Should().BeEquivalentTo([("/test-route", "GET")]);
    }

    [Fact]
    public void When_GetEndpointRequestDetailsAndMultipleMethodsArePresent_Expect_ListOfRouteAndMethodAreReturned()
    {
        AddEndpointRequest addEndpointRequest = new()
        {
            Route = "/test-route",
            Methods = ["GET", "POST", "PUT"]
        };

        var endpointRoutingDetails = addEndpointRequest.GetEndpointRoutingDetails();

        endpointRoutingDetails.Should().BeEquivalentTo([
            ("/test-route", "GET"),
            ("/test-route", "POST"),
            ("/test-route", "PUT")
        ]);
    }
}