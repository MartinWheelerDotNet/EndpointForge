using System.Text.Json.Serialization;
using EndpointForge.WebApi.Extensions;
using EndpointManager.Abstractions.Interfaces;
using EndpointManager.Abstractions.Models;
using Microsoft.AspNetCore.Http.Json;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<JsonOptions>(options =>
{
    options.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
});

builder.Services.AddEndpointForge();
builder.Services.AddOpenApi();
builder.AddServiceDefaults();

var application = builder.Build();

application.MapDefaultEndpoints();
application.UseEndpointForge();

if (application.Environment.IsDevelopment())
{
    application.MapOpenApi();
}

application.UseHttpsRedirection();

application.MapPost(
        "/add-endpoint",
        async (IEndpointForgeManager endpointManager, HttpRequest httpRequest)
            => await endpointManager.TryAddEndpointAsync(httpRequest))
    .Accepts<AddEndpointRequest>(contentType: "application/json")
    .Produces<AddEndpointRequest>(StatusCodes.Status201Created)
    .Produces<ErrorResponse>(StatusCodes.Status400BadRequest)
    .Produces<ErrorResponse>(StatusCodes.Status409Conflict)
    .Produces<ErrorResponse>(StatusCodes.Status422UnprocessableEntity);

application.Run();