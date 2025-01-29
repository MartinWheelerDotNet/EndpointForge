using System.Reflection;
using Xunit.v3;

namespace EndpointForge.WebApi.Tests.Attributes;

[ExcludeFromCodeCoverage]
public class StringNullEmptyOrWhitespaceInlineDataAttribute : DataAttribute
{ 
    public override ValueTask<IReadOnlyCollection<ITheoryDataRow>> GetData(
        MethodInfo testMethod, 
        DisposalTracker disposalTracker)
    => new(new List<TheoryDataRow<string?, bool>>
        {
            new(null, false),
            new("", false),
            new(" ", false),
            new("\t", false)
        });

    public override bool SupportsDiscoveryEnumeration() => true;

}