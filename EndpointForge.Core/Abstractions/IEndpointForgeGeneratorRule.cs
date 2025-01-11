namespace EndpointForge.Core.Abstractions;

public interface IEndpointForgeGeneratorRule : IEndpointForgeRule
{
    void Invoke(StreamWriter streamWriter);
}