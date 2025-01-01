using EndpointForge.Abstractions.Interfaces;
using EndpointForge.WebApi.Rules;
using EndpointForge.WebApi.Tests.Fakes;
using FluentAssertions;

namespace EndpointForge.WebApi.Tests.Rules;

public class GenerateGuidRuleTests
{
    [Fact]
    public void GenerateGuidRulePlaceholderIsGenerateGuid()
    {
        var stubGuidGenerator = new FakeGuidGenerator(Guid.Empty);
        var stubGuidWriter = new FakeGuidWriter();
        var generateGuidRule = new GenerateGuidRule(stubGuidGenerator, stubGuidWriter);
        
        generateGuidRule.Placeholder.Should().Be("generate:guid");
    }
    
    [Fact]
    public async Task When_Invoke_TheProvidedStreamShouldBeTheProvidedGuid()
    {
        var guid = Guid.NewGuid();
        var stubGuidGenerator = new FakeGuidGenerator(guid);
        var stubGuidWriter = new FakeGuidWriter();
        await using var streamWriter = new StreamWriter(new MemoryStream());
        
        var generateGuidRule = new GenerateGuidRule(stubGuidGenerator, stubGuidWriter);
        generateGuidRule.Invoke(streamWriter);
        
        await streamWriter.FlushAsync();
        streamWriter.BaseStream.Seek(0, SeekOrigin.Begin);
        
        using var streamReader = new StreamReader(streamWriter.BaseStream);
        var generatedGuid = await streamReader.ReadToEndAsync();
        
        generatedGuid.Should().Be(guid.ToString());
    }
}

public class FakeGuidWriter : IGuidWriter
{
    public void WriteGuidToStream(Guid guid, StreamWriter streamWriter) => streamWriter.Write(guid.ToString());

}