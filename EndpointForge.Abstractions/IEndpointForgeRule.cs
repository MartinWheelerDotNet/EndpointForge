namespace EndpointForge.Abstractions;

public interface IEndpointForgeRule
{
    public string Instruction { get; }
    public string Type { get; }
}