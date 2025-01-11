using EndpointForge.Models;
using EndpointForge.WebApi.Tests.Attributes;
using EndpointForge.WebApi.Validators;
using FluentAssertions;
using FluentValidation.TestHelper;

namespace EndpointForge.WebApi.Tests.Validators;

// Its recommend to supply a real validator instance instead of mocking, and supply it with bad data if required.
// https://docs.fluentvalidation.net/en/latest/testing.html#mocking
public class AddEndpointRequestValidatorTests
    {
        private readonly AddEndpointRequestValidator _endpointRequestValidator;

        public AddEndpointRequestValidatorTests()
        {
            var parameterValidator = new EndpointParameterDetailsValidator();
            _endpointRequestValidator = new AddEndpointRequestValidator(parameterValidator);
        }

        #region Route Tests
        [Theory]
        [InlineData("/test-route/{id:int}")]
        [InlineData("/test-route/test-index")]
        [InlineData("/test-route/{test-parameter}")]
        public void When_RouteIsValid_Expect_NoValidationError(string route)
        {
            var model = new AddEndpointRequest
            {
                Route = route,
                Methods = []
            };

            var result = _endpointRequestValidator.TestValidate(model);

            result.ShouldNotHaveValidationErrorFor(x => x.Route);
        }

        [Theory]
        [InlineData("/test-route/{id:in")]
        [InlineData("//test-route/test-index")]
        [InlineData("//test-route")]
        public void When_RouteIsInvalid_Expect_ValidationError(string route)
        {
            var model = new AddEndpointRequest
            {
                Route = route,
                Methods = ["GET"]
            };
            
            var result = _endpointRequestValidator.TestValidate(model);
            
            result.ShouldHaveValidationErrorFor(x => x.Route)
                .WithErrorMessage($"Endpoint request `route` is an invalid route: {route}.");
            
        }
        
        [Theory]
        [StringEmptyOrWhitespaceInlineData]
        public void When_RouteIsEmpty_Expect_ValidationError(string route)
        {
            var model = new AddEndpointRequest
            {
                Route = route,
                Methods = []
            };
            
            var result = _endpointRequestValidator.TestValidate(model);
            
            result.ShouldHaveValidationErrorFor(x => x.Route)
                .WithErrorMessage("Endpoint request `route` is empty or whitespace.");
        }

        #endregion

        #region Methods Tests
        [Theory]
        [InlineData("GET")]
        [InlineData("POST", "PUT")]
        [InlineData("DELETE")]
        public void When_MethodsAreValid_Expect_NoValidationError(params string[] methods)
        {
            var model = new AddEndpointRequest
            {
                Route = "/test-route",
                Methods = methods.ToList()
            };

            var result = _endpointRequestValidator.TestValidate(model);

            result.ShouldNotHaveValidationErrorFor(x => x.Methods);
        }

        [Fact]
        public void When_MethodsAreEmpty_Expect_ValidationError()
        {
            var model = new AddEndpointRequest
            {
                Route = "/test-route",
                Methods = []
            };
            
            var result = _endpointRequestValidator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(x => x.Methods)
                  .WithErrorMessage("Endpoint request `methods` contains no entries.");
        }
        #endregion

        #region Parameters Tests

        [Fact]
        public void When_ParametersAreValid_Expect_NoValidationError()
        {
            var validParameter = new EndpointParameterDetails("static", "test-parameter-name", "test-value");
            
            var model = new AddEndpointRequest
            {
                Route = "/test-route",
                Methods = ["GET"],
                Parameters = [validParameter]
            };
            
            var result = _endpointRequestValidator.TestValidate(model);

            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void When_ParametersAreInvalid_Expect_ValidationError()
        {
            var invalidParameter = new EndpointParameterDetails("invalid-type", "", "");

            var model = new AddEndpointRequest
            {
                Route = "/test-route",
                Methods = ["GET"],
                Parameters = [invalidParameter]
            };
            
            var result = _endpointRequestValidator.TestValidate(model);

            result.Should().NotBeNull();
            
            result.Errors.Should().HaveCount(3);
            result.ShouldHaveValidationErrorFor("Parameters[0].Type")
                .WithErrorMessage("Parameter `Type` is not valid (invalid-type).");
            result.ShouldHaveValidationErrorFor("Parameters[0].Name")
                .WithErrorMessage("Parameter `Name` cannot be empty.");
            result.ShouldHaveValidationErrorFor("Parameters[0].Value")
                .WithErrorMessage("Parameter `Value` cannot be empty.");
        }
        #endregion
    }