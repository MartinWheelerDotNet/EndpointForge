using EndpointForge.Abstractions.Models;
using FluentValidation;

namespace EndpointForge.WebApi.Validators;

public class EndpointForgeParameterDetailsValidator : AbstractValidator<EndpointForgeParameterDetails>
{
    private readonly List<string> _types = ["static", "header"];

    public EndpointForgeParameterDetailsValidator()
    {
        RuleFor(p => p.Type)
            .Must(BeAValidType)
            .WithMessage(details => $"Parameter `Type` is not valid ({details.Type}).");
        RuleFor(p => p.Name)
            .NotEmpty()
            .WithMessage("Parameter `Name` cannot be empty.");
        RuleFor(p => p.Value)
            .NotEmpty()
            .WithMessage("Parameter `Value` cannot be empty.");
    }
    private bool BeAValidType(string type) => _types.Contains(type);
}