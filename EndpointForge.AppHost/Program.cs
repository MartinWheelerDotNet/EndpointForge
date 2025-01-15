var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.EndpointForge_WebApi>("EndpointForge")
    .WithExternalHttpEndpoints();

builder.Build().Run();