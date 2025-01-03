namespace EndpointForge.Abstractions.Interfaces;

public interface IEndpointForgeInsertRule : IEndpointForgeRule
{
    void Invoke(StreamWriter streamWriter, ReadOnlySpan<char> value);
}