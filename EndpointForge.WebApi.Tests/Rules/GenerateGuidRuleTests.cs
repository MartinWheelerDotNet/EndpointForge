using EndpointForge.Core.Abstractions;
using EndpointForge.WebApi.Rules;
using FluentAssertions;
using Moq;

namespace EndpointForge.WebApi.Tests.Rules;

public class GenerateGuidRuleTests
{
    private readonly Mock<IGuidGenerator> _stubGuidGenerator = new();
    [Fact]
    public void GenerateGuidRuleInstructionShouldBeGenerate()
    {
        var generateGuidRule = new GenerateGuidRule(_stubGuidGenerator.Object);
        
        generateGuidRule.Instruction.Should().Be("generate");
    }
    
    [Fact]
    public void GenerateGuidRuleTypeShouldBeGuid()
    {
        var generateGuidRule = new GenerateGuidRule(_stubGuidGenerator.Object);
        
        generateGuidRule.Type.Should().Be("guid");
    }
    
    [Fact]
    public async Task When_Invoke_Expect_TheProvidedStreamShouldContainTheProvidedGuid()
    {
        var guid = Guid.NewGuid();
        _stubGuidGenerator.Setup(generator => generator.New).Returns(guid);
        
        await using var streamWriter = new StreamWriter(new MemoryStream());
        
        var generateGuidRule = new GenerateGuidRule(_stubGuidGenerator.Object);
        generateGuidRule.Invoke(streamWriter);
        
        await streamWriter.FlushAsync();
        streamWriter.BaseStream.Seek(0, SeekOrigin.Begin);
        using var streamReader = new StreamReader(streamWriter.BaseStream);
        var generatedGuid = await streamReader.ReadToEndAsync();
        
        generatedGuid.Should().Be(guid.ToString());
    }
}
