using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Xunit.Sdk;

namespace EndpointForge.Abstractions.Tests.Extensions;

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