using EndpointForge.Abstractions;
using EndpointForge.WebApi.Rules;
using FluentAssertions;
using Moq;

namespace EndpointForge.WebApi.Tests.Rules;

public class GenerateGuidRuleTests
{
    private readonly Mock<IGuidGenerator> _stubGuidGenerator = new();
    
    [Fact]
    public void GenerateGuidRuleTypeShouldBeGuid()
    {
        var generateGuidRule = new GenerateGuidRule(_stubGuidGenerator.Object);
        
        generateGuidRule.Type.Should().Be("guid");
    }
    
    [Fact]
    public async Task When_TryInvokeAndGuidIsGenerated_Expect_ReturnsTrue()
    {
        var guid = Guid.NewGuid();
        _stubGuidGenerator.Setup(generator => generator.New).Returns(guid);
        
        await using var streamWriter = new StreamWriter(new MemoryStream());
        
        var generateGuidRule = new GenerateGuidRule(_stubGuidGenerator.Object);
        var result = generateGuidRule.TryInvoke(streamWriter, out _);
        
        result.Should().BeTrue();
    }
    
    [Fact]
    public async Task When_Invoke_Expect_TheProvidedStreamShouldContainTheProvidedGuid()
    {
        var guid = Guid.NewGuid();
        _stubGuidGenerator.Setup(generator => generator.New).Returns(guid);
        
        await using var streamWriter = new StreamWriter(new MemoryStream());
        
        var generateGuidRule = new GenerateGuidRule(_stubGuidGenerator.Object);
        generateGuidRule.TryInvoke(streamWriter, out _);
        
        await streamWriter.FlushAsync();
        streamWriter.BaseStream.Seek(0, SeekOrigin.Begin);
        using var streamReader = new StreamReader(streamWriter.BaseStream);
        var generatedGuid = await streamReader.ReadToEndAsync();
        
        generatedGuid.Should().Be(guid.ToString());
    }
    
    [Fact]
    public async Task When_TryInvokeAndGuidIsGenerated_Expect_OutValueIsAGuid()
    {
        var guid = Guid.NewGuid();
        _stubGuidGenerator.Setup(generator => generator.New).Returns(guid);
        
        await using var streamWriter = new StreamWriter(new MemoryStream());
        
        var generateGuidRule = new GenerateGuidRule(_stubGuidGenerator.Object);
        generateGuidRule.TryInvoke(streamWriter, out var generatedGuid);
        
        var parsedGuid = Guid.Parse(generatedGuid);
        
        parsedGuid.Should().Be(guid);
    }
}
