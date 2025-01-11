using EndpointForge.Models;
using EndpointForge.WebApi.Tests.Attributes;
using EndpointForge.WebApi.Validators;
using FluentValidation.TestHelper;

namespace EndpointForge.WebApi.Tests.Validators;

public class EndpointParameterDetailsValidatorTests
{
    private readonly EndpointParameterDetailsValidator _validator = new();

    #region Name Tests
    [Fact]
    public void When_NameIsNotEmpty_Expect_NoValidationError()
    {
        var model = new EndpointParameterDetails("","test-parameter-name", "");

        var result = _validator.TestValidate(model);
        
        result.ShouldNotHaveValidationErrorFor(p => p.Name);
    }
    
    [Theory]
    [StringEmptyOrWhitespaceInlineData]
    public void When_NameIsEmpty_Expect_ValidationError(string name)
    {
        var model = new EndpointParameterDetails("", name, "");

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(p => p.Name)
            .WithErrorMessage("Parameter `Name` cannot be empty.");
    }
    #endregion
    
    #region Type Tests
    [Theory]
    [InlineData("static")]
    [InlineData("header")]
    public void When_TypeIsValid_Expect_NoValidationError(string type)
    {
        var details = new EndpointParameterDetails(type, "", "");

        var result = _validator.TestValidate(details);

        result.ShouldNotHaveValidationErrorFor(p => p.Type);
    }
    
    [Theory]
    [InlineData("query")]
    [InlineData("body")]
    [StringEmptyOrWhitespaceInlineData]
    public void When_TypeIsInvalid_Expect_ValidationError(string type)
    {
        var model = new EndpointParameterDetails(type, "", "");

        var result = _validator.TestValidate(model);
        
        result.ShouldHaveValidationErrorFor(p => p.Type)
            .WithErrorMessage($"Parameter `Type` is not valid ({type}).");
    }
    #endregion
    
    #region Value Tests
    [Fact]
    public void When_ValueIsNotEmpty_Expect_NoValidationError()
    {
        var model = new EndpointParameterDetails("","", "test-value");

        var result = _validator.TestValidate(model);
        
        result.ShouldNotHaveValidationErrorFor(p => p.Value);
    }
    
    [Theory]
    [StringEmptyOrWhitespaceInlineData]
    public void When_ValueIsEmpty_Expect_ValidationError(string value)
    {
        var model = new EndpointParameterDetails("", "", value);

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(p => p.Value)
            .WithErrorMessage("Parameter `Value` cannot be empty.");
    }
    #endregion
}


