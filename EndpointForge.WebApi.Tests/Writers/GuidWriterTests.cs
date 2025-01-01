using EndpointForge.WebApi.Writers;
using FluentAssertions;

namespace EndpointForge.WebApi.Tests.Writers;

public class GuidWriterTests
{
    [Fact]
    public async Task When_WriteGuidToStream_Then_StreamContainsProvidedGuid()
    {
        var guid = Guid.NewGuid();
        var guidWriter = new GuidWriter();
        
        await using var streamWriter = new StreamWriter(new MemoryStream());
        guidWriter.WriteGuidToStream(guid, streamWriter);
        
        await streamWriter.FlushAsync();
        streamWriter.BaseStream.Seek(0, SeekOrigin.Begin);
        using var streamReader = new StreamReader(streamWriter.BaseStream);
        var result = await streamReader.ReadToEndAsync();
        
        result.Should().Be(guid.ToString());
    }
}