using EndpointForge.Models;
using FluentValidation;

namespace EndpointForge.WebApi.Validators;

public class AddEndpointRequestValidator : AbstractValidator<AddEndpointRequest>
{
    public AddEndpointRequestValidator(IValidator<EndpointParameterDetails> endpointForgeParameterDetailsValidator)
    {
        RuleFor(x => x.Route)
            .NotEmpty()
            .WithMessage(details => $"Endpoint request `{nameof(details.Route)}` is empty or whitespace.")
            .Must(BeAValidRoute)
            .WithMessage(details => $"Endpoint request `{nameof(details.Route)}` is an invalid route: {details.Route}.");

        RuleFor(x => x.Methods)
            .NotEmpty()
            .WithMessage(details => $"Endpoint request `{nameof(details.Methods)}` contains no entries.");

        RuleForEach(x => x.Parameters)
            .SetValidator(endpointForgeParameterDetailsValidator);
    }

    private bool BeAValidRoute(string route)
    {
        try
        {
            _ = RoutePatternFactory.Parse(route);
            return true;
        }
        catch
        {
            return false;
        }
    }
}