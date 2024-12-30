using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace EndpointForge.WebApi.Tests.Fakes;

[ExcludeFromCodeCoverage]
internal class FakeEndpointRouteBuilder : IEndpointRouteBuilder
{
    public IApplicationBuilder CreateApplicationBuilder() 
        => new ApplicationBuilder(ServiceProvider);

    public IServiceProvider ServiceProvider { get; set; } = null!;
    public ICollection<EndpointDataSource> DataSources { get; } = new List<EndpointDataSource>();
}