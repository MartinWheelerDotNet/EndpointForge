using EndpointForge.WebApi.Rules;
using EndpointForge.WebApi.Tests.Fakes;
using FluentAssertions;

namespace EndpointForge.WebApi.Tests.Rules;

public class GenerateGuidRuleTests
{
    [Fact]
    public void GenerateGuidRuleInstructionShouldBeGenerate()
    {
        var stubGuidGenerator = new FakeGuidGenerator(Guid.Empty);
        var generateGuidRule = new GenerateGuidRule(stubGuidGenerator);
        
        generateGuidRule.Instruction.Should().Be("generate");
    }
    
    [Fact]
    public void GenerateGuidRuleTypeShouldBeGuid()
    {
        var stubGuidGenerator = new FakeGuidGenerator(Guid.Empty);
        var generateGuidRule = new GenerateGuidRule(stubGuidGenerator);
        
        generateGuidRule.Type.Should().Be("guid");
    }
    
    [Fact]
    public async Task When_Invoke_Expect_TheProvidedStreamShouldContainTheProvidedGuid()
    {
        var guid = Guid.NewGuid();
        var stubGuidGenerator = new FakeGuidGenerator(guid);
        await using var streamWriter = new StreamWriter(new MemoryStream());
        
        var generateGuidRule = new GenerateGuidRule(stubGuidGenerator);
        generateGuidRule.Invoke(streamWriter);
        
        await streamWriter.FlushAsync();
        streamWriter.BaseStream.Seek(0, SeekOrigin.Begin);
        
        using var streamReader = new StreamReader(streamWriter.BaseStream);
        var generatedGuid = await streamReader.ReadToEndAsync();
        
        generatedGuid.Should().Be(guid.ToString());
    }
}
