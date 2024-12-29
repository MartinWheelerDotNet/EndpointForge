using System.Net;

namespace EndpointManager.Abstractions.Models;

public record ErrorResponse(HttpStatusCode StatusCode, IEnumerable<string> Errors)
{ 
    public override string ToString() => $"StatusCode={StatusCode}, Errors=[{string.Join(", ", Errors)}]";
}
