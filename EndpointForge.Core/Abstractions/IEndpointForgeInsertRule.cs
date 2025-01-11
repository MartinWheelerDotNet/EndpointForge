namespace EndpointForge.Core.Abstractions;

public interface IEndpointForgeInsertRule : IEndpointForgeRule
{
    void Invoke(StreamWriter streamWriter, ReadOnlySpan<char> value);
}