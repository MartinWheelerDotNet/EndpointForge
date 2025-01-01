using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using EndpointForge.WebApi.Extensions;
using EndpointForge.Abstractions.Interfaces;
using EndpointForge.Abstractions.Models;
using EndpointForge.ServiceDefaults;
using Microsoft.AspNetCore.Http.Json;

namespace EndpointForge.WebApi;

[ExcludeFromCodeCoverage]
internal class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        ConfigureDefaultServices(builder);
        
        var application = builder.Build();

        application.MapDefaultEndpoints();
        application.UseEndpointForge();

        if (application.Environment.IsDevelopment()) 
            application.MapOpenApi();
        
        application.UseHttpsRedirection();

        application.MapPost(
            "/add-endpoint",
            async (ILogger<Program> logger, IEndpointForgeManager endpointManager, HttpRequest httpRequest)
                => await AddEndpoint(logger, httpRequest, endpointManager))
            .Accepts<AddEndpointRequest>(contentType: "application/json")
            .Produces<AddEndpointRequest>(StatusCodes.Status201Created)
            .Produces<ErrorResponse>(StatusCodes.Status400BadRequest)
            .Produces<ErrorResponse>(StatusCodes.Status409Conflict)
            .Produces<ErrorResponse>(StatusCodes.Status422UnprocessableEntity);

        application.Run();
    }

    #region Endpoint Methods
    private static async Task<IResult> AddEndpoint(ILogger<Program> logger, HttpRequest httpRequest, IEndpointForgeManager endpointManager)
    {
        logger.LogInformation("Add endpoint request received.");
        var (addEndpointRequest, errorResponse) = await httpRequest.TryDeserializeRequestAsync<AddEndpointRequest>();

        if (errorResponse is null)
        {
            logger.LogInformation("Deserialized AddEndpointRequest.");
            return await endpointManager.TryAddEndpointAsync(addEndpointRequest!);
        } 
            
        logger.LogErrorResponse(errorResponse);
        return errorResponse.GetTypedResult();
    }
    #endregion

    private static void ConfigureDefaultServices(WebApplicationBuilder builder)
    {
        builder.Services.Configure<JsonOptions>(options =>
        {
            options.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        });

        builder.Services
            .AddEndpointForge()
            .AddOpenApi();
        
        builder.AddServiceDefaults();
    }
}