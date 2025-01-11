using System.Reflection;

namespace EndpointForge.WebApi.Tests.Attributes;

[ExcludeFromCodeCoverage]
public class StringNullEmptyOrWhitespaceInlineDataAttribute : DataAttribute
{
    public override IEnumerable<object?[]> GetData(MethodInfo testMethod)
    {
        return
        [
            [null, false],
            ["", false],
            [" ", false],
            ["\t", false],
            ["\n", false]
        ];
    }
}