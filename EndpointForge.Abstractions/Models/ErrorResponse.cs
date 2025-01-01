using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace EndpointForge.Abstractions.Models;

public record ErrorResponse(HttpStatusCode StatusCode, IEnumerable<string> Errors)
{ 
    [ExcludeFromCodeCoverage]
    public override string ToString() => $"StatusCode={StatusCode}, Errors=[{string.Join(", ", Errors)}]";
}
