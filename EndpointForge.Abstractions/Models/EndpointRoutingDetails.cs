using Microsoft.AspNetCore.Routing.Patterns;

namespace EndpointForge.Abstractions.Models;

[Serializable]
public record EndpointRoutingDetails(string Route, string Method);