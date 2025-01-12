using EndpointForge.WebApi.Rules;
using FluentAssertions;

namespace EndpointForge.WebApi.Tests.Rules;

public class InsertParameterRuleTests
{
    [Fact]
    public void When_NewInsertParameterRule_Expected_TypeIsParameter()
    {
        var insertParameterRule = new InsertParameterRule();

        insertParameterRule.Type.Should().Be("parameter");
    }
    
    [Fact]
    public async Task When_InvokeWithParameter_Expect_ParameterWrittenToStream()
    {
        await using var streamWriter = new StreamWriter(new MemoryStream());
        
        var insertParameterRule = new InsertParameterRule();
        insertParameterRule.Invoke(streamWriter, "value");
        
        await streamWriter.FlushAsync();
        streamWriter.BaseStream.Seek(0, SeekOrigin.Begin);
        
        using var streamReader = new StreamReader(streamWriter.BaseStream);
        var parameterValue = await streamReader.ReadToEndAsync();
        
        parameterValue.Should().Be("value");
    }
}