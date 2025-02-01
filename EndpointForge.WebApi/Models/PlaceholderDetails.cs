using EndpointForge.WebApi.Constants;

namespace EndpointForge.WebApi.Models;

public ref struct PlaceholderDetails
{
    public ReadOnlySpan<char> Rule;
    public ReadOnlySpan<char> Instruction;
    public ReadOnlySpan<char> Name;
    public ReadOnlySpan<char> Values;

    private PlaceholderDetails(
        ReadOnlySpan<char> rule, 
        ReadOnlySpan<char> instruction, 
        ReadOnlySpan<char> name,
        ReadOnlySpan<char> values)
    {
        Rule = rule;
        Instruction = instruction;
        Name = name;
        Values = values;
    }

    public static bool TryParse(ReadOnlySpan<char> placeholderSpan, out PlaceholderDetails details)
    {
        var placeholderSplitEnumerator = placeholderSpan.Split(':');
        details = default;
        
        if (!placeholderSplitEnumerator.MoveNext())
            return false;
        var rule = placeholderSpan[placeholderSplitEnumerator.Current];
        if (!RuleType.IsRule(rule))
        {
            return false;
        }
        
        if (!placeholderSplitEnumerator.MoveNext())
            return false;
        var instruction = placeholderSpan[placeholderSplitEnumerator.Current];
        if (!InstructionType.IsInstruction(instruction))
        {
            return false;
        }
        
        
        if (!placeholderSplitEnumerator.MoveNext())
        {
            details = new PlaceholderDetails(rule, instruction, ReadOnlySpan<char>.Empty, ReadOnlySpan<char>.Empty);
            return true;
        }
        var name = placeholderSpan[placeholderSplitEnumerator.Current];

        
        if (!placeholderSplitEnumerator.MoveNext())
        {
            details = new PlaceholderDetails(rule, instruction, name, ReadOnlySpan<char>.Empty);
            return true;
        }
        var values = placeholderSpan[placeholderSplitEnumerator.Current];
        
        details =  new PlaceholderDetails(rule, instruction, name, values);
        return true;
    }
}