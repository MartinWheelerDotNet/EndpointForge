namespace EndpointForge.Abstractions.Interfaces;

public interface IEndpointForgeGeneratorRule : IEndpointForgeRule
{
    void Invoke(StreamWriter streamWriter);
}