using EndpointForge.Abstractions.Models;
using FluentValidation;
using Microsoft.AspNetCore.Routing.Patterns;

namespace EndpointForge.WebApi.Validators;

public class AddEndpointRequestValidator : AbstractValidator<AddEndpointRequest>
{
    public AddEndpointRequestValidator(IValidator<EndpointForgeParameterDetails> endpointForgeParameterDetailsValidator)
    {
        RuleFor(x => x.Route)
            .NotEmpty()
            .WithMessage("Endpoint request `route` is empty or whitespace.")
            .Must(BeAValidRoute)
            .WithMessage(details => $"Endpoint request `route` is an invalid route: {details.Route}.");

        RuleFor(x => x.Methods)
            .NotEmpty()
            .WithMessage("Endpoint request `methods` contains no entries.");

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