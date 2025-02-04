using EndpointForge.WebApi.Extensions;
using EndpointForge.WebApi.Models;
using FluentAssertions;

namespace EndpointForge.WebApi.Tests.Extensions;

public class PlaceholderDetailsExtensionsTests
{
    #region IsRepeatStart Tests
    [Fact]
    public void When_IsRepeatStart_Expect_True()
    {
        _ = PlaceholderDetails.TryParse("repeat:start:repeat-name:1", out var placeholderDetails);

        var result = placeholderDetails.IsRepeatStart();

        result.Should().BeTrue();
    }
    
    [Fact]
    public void When_IsNotRepeater_Expect_False()
    {
        _ = PlaceholderDetails.TryParse("generate:guid", out var placeholderDetails);

        var result = placeholderDetails.IsRepeatStart();

        result.Should().BeFalse();
    }
    
    [Fact]
    public void When_IsRepeatEnd_Expect_False()
    {
        _ = PlaceholderDetails.TryParse("repeat:end:repeat-name", out var placeholderDetails);

        var result = placeholderDetails.IsRepeatStart();

        result.Should().BeFalse();
    }
    #endregion
    
    #region IsMatchingRepeatEnd Tests
    [Fact]
    public void When_IsMatchingRepeatEnd_Expect_True()
    {
        const string repeatName = "repeat-name";
        _ = PlaceholderDetails.TryParse("repeat:end:repeat-name", out var placeholderDetails);

        var result = placeholderDetails.IsMatchingRepeatEnd(repeatName);

        result.Should().BeTrue();
    }
    
    [Fact]
    public void When_IsRepeatStart_Expect_False()
    {
        const string repeatName = "repeat-name";
        _ = PlaceholderDetails.TryParse("repeat:start:repeat-name:1", out var placeholderDetails);

        var result = placeholderDetails.IsMatchingRepeatEnd(repeatName);

        result.Should().BeFalse();
    }
    
    [Fact]
    public void When_IsRepeatEndWithoutMatchingName_Expect_False()
    {
        const string repeatName = "repeat-name";
        _ = PlaceholderDetails.TryParse("repeat:end:other-repeat-name", out var placeholderDetails);

        var result = placeholderDetails.IsMatchingRepeatEnd(repeatName);

        result.Should().BeFalse();
    }
    #endregion
}