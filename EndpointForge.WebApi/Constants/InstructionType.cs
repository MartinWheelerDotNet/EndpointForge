namespace EndpointForge.WebApi.Constants;

public static class InstructionType
{
    public const string Start = "start";
    public const string End = "end";
    public const string Guid = "guid";
    public const string Parameter = "parameter";

    public static bool IsDefined(ReadOnlySpan<char> instruction)
        => instruction switch
        {
            Start or End => true,
            Guid => true,
            Parameter => true,
            _ => false
        };
}