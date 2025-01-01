namespace EndpointForge.Abstractions.Interfaces;

public interface IEndpointForgeGeneratorRule
{
    public string Placeholder { get; }
    void Invoke(StreamWriter streamWriter);
}