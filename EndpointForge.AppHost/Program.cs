var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.EndpointForge_WebApi>("webapi")
    .WithExternalHttpEndpoints();

builder.Build().Run();