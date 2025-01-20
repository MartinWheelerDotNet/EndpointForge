namespace EndpointForge.Abstractions;

public interface IEndpointForgeGeneratorRule : IEndpointForgeRule
{
    bool TryInvoke(StreamWriter streamWriter, out ReadOnlySpan<char> generatedValue);
}