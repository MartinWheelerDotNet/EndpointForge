using EndpointForge.WebApi.Exceptions;
using EndpointForge.WebApi.Extensions;
using FluentAssertions;

namespace EndpointForge.WebApi.Tests.Extensions;

public class ReadOnlySpanExtensionsTests
{
    #region IsStartOfPlaceholderBegin
    [Fact]
    public void When_IsStartOfPlaceholderBeginAndIsValidInstruction_Expect_True()
    {
        var isStartOfPlaceholderBegin = "{{rule:instruction}}".AsSpan().IsStartOfPlaceholderBegin(0);

        isStartOfPlaceholderBegin.Should().BeTrue();
    }

    [Fact]
    public void When_IsStartOfPlaceholderBeginAndPlaceholderContentIsEmpty_Expect_False()
    {
        var isStartOfPlaceholderBegin = "{{}}".AsSpan().IsStartOfPlaceholderBegin(0);

        isStartOfPlaceholderBegin.Should().BeFalse();
    }
    
    [Fact]
    public void When_IsStartOfPlaceholderBeginAndPlaceholderContentReachesEndOfSpan_Expect_False()
    {
        var isStartOfPlaceholderBegin = "{{a".AsSpan().IsStartOfPlaceholderBegin(0);

        isStartOfPlaceholderBegin.Should().BeFalse();
    }
    
    [Fact]
    public void When_IsStartOfPlaceholderBeginAndAtEndOfStream_Expect_False()
    {
        var isStartOfPlaceholderBegin = "{".AsSpan().IsStartOfPlaceholderBegin(0);

        isStartOfPlaceholderBegin.Should().BeFalse();
    }
    
    [Fact]
    public void When_IsStartOfPlaceholderBeginAndAtOnlyHasOnePlaceholderCharacter_Expect_False()
    {
        var isStartOfPlaceholderBegin = "{[".AsSpan().IsStartOfPlaceholderBegin(0);

        isStartOfPlaceholderBegin.Should().BeFalse();
    }
    #endregion
    
    #region IsStartOfPlaceholderEnd
    [Fact]
    public void When_IsStartOfPlaceholderEnd_Expect_True()
    {
        var isStartOfPlaceholderBegin = "}}".AsSpan().IsStartOfPlaceholderEnd(0);

        isStartOfPlaceholderBegin.Should().BeTrue();
    }
    
    [Fact]
    public void When_IsStartOfPlaceholderEndAndAtEndOfStream_Expect_False()
    {
        var isStartOfPlaceholderBegin = "}".AsSpan().IsStartOfPlaceholderEnd(0);

        isStartOfPlaceholderBegin.Should().BeFalse();
    }
    
    [Fact]
    public void When_IsStartOfPlaceholderEndAndAtOnlyHasOnePlaceholderCharacter_Expect_False()
    {
        var isStartOfPlaceholderBegin = "}]".AsSpan().IsStartOfPlaceholderEnd(0);

        isStartOfPlaceholderBegin.Should().BeFalse();
    }
    #endregion
    
    #region ExtractPlaceholder

    [Fact]
    public void When_ExtractPlaceholderContentAndPlaceholderHasContent_ExpectContentAndReadPositionIsSetToEndOfPlaceholder()
    {
        const string placeholder = "{{generate:guid}}";
        const string expectedPlaceholderContent = "generate:guid";
        var readPosition = 2;
        var placeholderContent = placeholder.AsSpan().ExtractPlaceholder(ref readPosition);
        
        placeholderContent.ToString().Should().Be(expectedPlaceholderContent);
        readPosition.Should().Be(placeholder.Length);
    }
    
    [Fact]
    public void When_ExtractPlaceholderContentAndPlaceholderHasNoContent_ExpectEmptyContentIdAndReadPositionSetToEndOfPlaceholder()
    {
        var readPosition = 2;
        var placeholder = "{{}}".AsSpan().ExtractPlaceholder(ref readPosition);
        
        placeholder.IsEmpty.Should().BeTrue();
        readPosition.Should().Be(4);
    }

    [Fact]
    public void When_ExtractPlaceholderContentAndPlaceholderHasNoEndMarker_ExpectExceptionThrown()
    {
        var readPosition = 2;
        
        var exception = Assert.Throws<InvalidPlaceholderException>(() =>
        {
            "{{generate:guid".AsSpan().ExtractPlaceholder(ref readPosition);
        });

        Assert.Multiple(
                () => exception.Message.Should().Be(
                    "An invalid placeholder was found at position [0] in the response body"),
                () => exception.Errors.Should().BeEquivalentTo(
                    "Placeholder end marker not found"));
    }
    
    [Fact]
    public void When_ExtractPlaceholderContentAndNewStartMarkerFoundBeforeEndMarker_ExpectExceptionThrown()
    {
        var readPosition = 2;
        
        var exception = Assert.Throws<InvalidPlaceholderException>(() =>
        {
            "{{generate:guid{{generate:guid}}".AsSpan().ExtractPlaceholder(ref readPosition);
        });

        Assert.Multiple(
            () => exception.Message.Should().Be(
                "An invalid placeholder was found at position [0] in the response body"),
            () => exception.Errors.Should().BeEquivalentTo(
                "New placeholder start marker found at position [15] before the end of current placeholder"));
    }
    
    #endregion
}