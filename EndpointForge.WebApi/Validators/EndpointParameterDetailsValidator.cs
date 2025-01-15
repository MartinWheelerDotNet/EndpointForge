using EndpointForge.Abstractions.Constants;
using EndpointForge.Models;
using FluentValidation;

namespace EndpointForge.WebApi.Validators;

public class EndpointParameterDetailsValidator : AbstractValidator<EndpointParameterDetails>
{
    private readonly List<string> _types = [ ParameterType.Header, ParameterType.Static ];

    public EndpointParameterDetailsValidator()
    {
        RuleFor(p => p.Type)
            .Must(BeAValidType)
            .WithMessage(details => $"Parameter `{nameof(details.Type)}` is not valid ({details.Type}).");
        RuleFor(p => p.Name)
            .NotEmpty()
            .WithMessage(details => $"Parameter `{nameof(details.Name)}` cannot be empty.");
        RuleFor(p => p.Value)
            .NotEmpty()
            .WithMessage(details => $"Parameter `{nameof(details.Value)}` cannot be empty.");
    }
    private bool BeAValidType(string type) => _types.Contains(type);
}