using EndpointForge.Models;

namespace EndpointForge.WebApi.Extensions;

public static class EndpointRoutingDetailsExtensions
{
    public static IEnumerable<(string Route, string Method)> GetEndpointRoutingDetails(
        this AddEndpointRequest addEndpointRequest) 
        => addEndpointRequest.Methods.Select(method => (addEndpointRequest.Route, method));
}