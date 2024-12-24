using EndpointManager.Abstractions.Interfaces;
using EndpointManager.Abstractions.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IEndpointManager, EndpointForge.WebApi.Managers.EndpointManager>();
builder.Services.AddOpenApi();
builder.AddServiceDefaults();

var application = builder.Build();

application.MapDefaultEndpoints();

if (application.Environment.IsDevelopment())
{
    application.MapOpenApi();
}


application.UseHttpsRedirection();

application.MapPost(
        "/add-endpoint",
        async (IEndpointManager endpointManager, HttpRequest httpRequest)
            => await endpointManager.TryAddEndpointAsync(httpRequest))
    .Accepts<AddEndpointRequest>(contentType: "application/json")
    .Produces<AddEndpointRequest>(StatusCodes.Status201Created)
    .Produces<ErrorResponse>(StatusCodes.Status400BadRequest)
    .Produces<ErrorResponse>(StatusCodes.Status409Conflict)
    .Produces<ErrorResponse>(StatusCodes.Status422UnprocessableEntity);

application.Run();