namespace EndpointForge.WebApi.Constants;

public static class RuleType
{
    public const string Insert = "insert";
    public const string Repeat = "repeat";
    public const string Generate = "generate";

    public static bool IsDefined(ReadOnlySpan<char> ruleType)
        => ruleType switch
        {
            Repeat => true,
            Generate => true,
            Insert => true,
            _ => false
        };

}