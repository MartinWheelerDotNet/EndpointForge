using EndpointForge.Abstractions;

namespace EndpointForge.WebApi.Rules;

public class InsertParameterRule : IEndpointForgeInsertRule
{
    private const string RuleType = "parameter";
    public string Type => RuleType;

    public void Invoke(StreamWriter streamWriter, ReadOnlySpan<char> value) => streamWriter.Write(value);
}