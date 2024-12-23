using System.Text.Json;
using EndpointManager.Abstractions.Interfaces;
using EndpointManager.Abstractions.Models;
using IdentityModel.OidcClient;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Http.HttpResults;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IEndpointManager, EndpointForge.WebApi.Managers.EndpointManager>();
builder.Services.AddOpenApi();
builder.AddServiceDefaults();

var app = builder.Build();

app.MapDefaultEndpoints();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}


app.UseHttpsRedirection();

app.MapPost(
    "/add-endpoint",
    async Task<Results<
            Created<AddEndpointRequest>,
            UnprocessableEntity<ErrorResponse>, 
            Conflict<ErrorResponse>,
            BadRequest<ErrorResponse>>> 
        (IEndpointManager endpointManager, HttpRequest httpRequest)
        => await endpointManager.TryAddEndpointAsync(httpRequest));

app.Run();