using System.Diagnostics.CodeAnalysis;
using EndpointForge.Abstractions.Interfaces;

namespace EndpointForge.WebApi.Tests.Fakes
{
    [ExcludeFromCodeCoverage]
    internal record FakeResponseBodyParser(string Body) : IResponseBodyParser
    {
        public async Task ProcessResponseBody(
            Stream stream, 
            string responseBody, 
            Dictionary<string, string> parameters)
        {
            stream.Seek(0, SeekOrigin.Begin);
            await using var streamWriter = new StreamWriter(stream, leaveOpen: true);
            await streamWriter.WriteAsync(Body);
            await streamWriter.FlushAsync();
        }
    }
}