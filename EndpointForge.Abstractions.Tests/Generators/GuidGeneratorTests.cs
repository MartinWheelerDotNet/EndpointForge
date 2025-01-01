using EndpointForge.Abstractions.Generators;
using FluentAssertions;

namespace EndpointForge.Abstractions.Tests.Generators;

public class GuidGeneratorTests
{
    [Fact]
    public void When_New_Expect_Guid_Is_Not_EmptyGuid()
    {
        var guid = new GuidGenerator().New();
        
        guid.Should().NotBeEmpty();
    }
}