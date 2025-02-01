using EndpointForge.Abstractions;
using EndpointForge.WebApi.Constants;

namespace EndpointForge.WebApi.Rules;

public class InsertParameterRule : IEndpointForgeInsertRule
{
    public string Type => InstructionType.Parameter;

    public void Invoke(StreamWriter streamWriter, ReadOnlySpan<char> value) => streamWriter.Write(value);
}