using System.Reflection;
using Xunit.v3;

namespace EndpointForge.WebApi.Tests.Attributes;

[ExcludeFromCodeCoverage]
public class StringEmptyOrWhitespaceInlineDataAttribute : DataAttribute
{
    public override ValueTask<IReadOnlyCollection<ITheoryDataRow>> GetData(
        MethodInfo testMethod,
        DisposalTracker disposalTracker)
        => new(new List<TheoryDataRow<string>> { "", " ", "\t", "\n" });

    public override bool SupportsDiscoveryEnumeration() => true;
}