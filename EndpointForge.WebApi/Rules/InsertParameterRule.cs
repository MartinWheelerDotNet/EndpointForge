using EndpointForge.Core.Abstractions;

namespace EndpointForge.WebApi.Rules;

public class InsertParameterRule : IEndpointForgeInsertRule
{
    private const string RuleInstruction = "insert";
    private const string RuleType = "parameter";
    public string Instruction => RuleInstruction;
    public string Type => RuleType;

    public void Invoke(StreamWriter streamWriter, ReadOnlySpan<char> value) => streamWriter.Write(value);
}