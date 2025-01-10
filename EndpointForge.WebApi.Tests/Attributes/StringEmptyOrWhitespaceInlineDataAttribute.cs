using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Xunit.Sdk;

namespace EndpointForge.WebApi.Tests.Attributes;

[ExcludeFromCodeCoverage]
public class StringEmptyOrWhitespaceInlineDataAttribute : DataAttribute
{
    public override IEnumerable<object[]> GetData(MethodInfo testMethod) => [[""], [" "], ["\t"], ["\n"]];
}