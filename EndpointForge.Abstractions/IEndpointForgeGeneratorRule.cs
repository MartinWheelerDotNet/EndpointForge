namespace EndpointForge.Abstractions;

public interface IEndpointForgeGeneratorRule : IEndpointForgeRule
{
    void Invoke(StreamWriter streamWriter);
}