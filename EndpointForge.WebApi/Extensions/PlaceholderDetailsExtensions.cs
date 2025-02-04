using EndpointForge.WebApi.Constants;
using EndpointForge.WebApi.Models;

namespace EndpointForge.WebApi.Extensions;

public static class PlaceholderDetailsExtensions
{
    public static bool IsRepeatStart(this PlaceholderDetails details)
        => details is { Rule: RuleType.Repeat, Instruction: InstructionType.Start };

    public static bool IsMatchingRepeatEnd(this PlaceholderDetails details, ReadOnlySpan<char> name)
        => details is { Rule: RuleType.Repeat, Instruction: InstructionType.End }
           && details.Name.SequenceEqual(name);
}