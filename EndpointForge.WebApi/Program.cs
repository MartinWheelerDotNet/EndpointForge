using System.Text.Json.Serialization;
using EndpointForge.Abstractions;
using EndpointForge.Models;
using EndpointForge.WebApi.Extensions;
using EndpointForge.ServiceDefaults;
using EndpointForge.WebApi.Middleware;
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
        application.UseMiddleware<ExceptionHandlingMiddleware>();
        
        if (application.Environment.IsDevelopment()) 
            application.MapOpenApi();
        
        application.UseHttpsRedirection();

        application.MapPost(
            "/add-endpoint",
            async (ILogger<Program> logger, IEndpointForgeManager endpointManager, HttpRequest httpRequest) => 
            {
                logger.LogInformation("Add endpoint request received.");
                
                var addEndpointRequest = await httpRequest.TryDeserializeRequestAsync<AddEndpointRequest>();
                
                logger.LogInformation("Deserialized AddEndpointRequest.");
                return await endpointManager.TryAddEndpointAsync(addEndpointRequest);
            })
            .Accepts<AddEndpointRequest>(contentType: "application/json")
            .Produces(StatusCodes.Status201Created)
            .Produces<ErrorResponse>(StatusCodes.Status400BadRequest)
            .Produces<ErrorResponse>(StatusCodes.Status409Conflict)
            .Produces<ErrorResponse>(StatusCodes.Status422UnprocessableEntity);

        application.Run();
    }

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