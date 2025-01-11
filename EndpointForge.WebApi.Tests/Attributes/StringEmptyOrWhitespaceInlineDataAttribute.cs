using System.Reflection;

namespace EndpointForge.WebApi.Tests.Attributes;

[ExcludeFromCodeCoverage]
public class StringEmptyOrWhitespaceInlineDataAttribute : DataAttribute
{
    public override IEnumerable<object[]> GetData(MethodInfo testMethod) => [[""], [" "], ["\t"], ["\n"]];
}