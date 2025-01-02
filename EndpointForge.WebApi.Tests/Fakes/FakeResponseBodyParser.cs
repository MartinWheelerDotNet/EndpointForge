using System.Diagnostics.CodeAnalysis;
using EndpointForge.Abstractions.Interfaces;

namespace EndpointForge.WebApi.Tests.Fakes;

[ExcludeFromCodeCoverage]
internal class FakeResponseBodyParser(string body) : IResponseBodyParser
{
    public async Task ProcessResponseBody(Stream stream, string responseBody, IDictionary<string, string> parameters)
    {
        stream.Seek(0, SeekOrigin.Begin);
        var streamWriter = new StreamWriter(stream);
        await streamWriter.WriteAsync(body);
        await streamWriter.FlushAsync();
    }
}