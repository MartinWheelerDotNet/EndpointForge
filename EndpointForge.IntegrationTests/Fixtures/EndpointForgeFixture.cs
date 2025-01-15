using Aspire.Hosting;
using JetBrains.Annotations;
using Microsoft.Extensions.Hosting;

namespace EndpointForge.IntegrationTests.Fixtures;

// EndpointForgeFixture is used as a class fixture via DI and is not directly instantiated, however due to that XUnit
// class fixtures require a parameterless public constructor, it is not possible to make this class abstract.
[UsedImplicitly]
public class EndpointForgeFixture : IDisposable
{
    public readonly DistributedApplication Application;

    public EndpointForgeFixture()
    { 
        var appHost = CreateDefaultAppHost().Result;
        Application = appHost.BuildAsync().Result;
        Application.Start();
        WaitForRunningState(Application, "EndpointForge")
            .ConfigureAwait(ConfigureAwaitOptions.None)
            .GetAwaiter()
            .GetResult();
    }
    
    private static async Task<IDistributedApplicationTestingBuilder> CreateDefaultAppHost()
    {
        var appHost = await DistributedApplicationTestingBuilder.CreateAsync<Projects.EndpointForge_AppHost>();
        appHost.Services.ConfigureHttpClientDefaults(clientBuilder =>
        {
            clientBuilder.AddStandardResilienceHandler();
        });
        
        return appHost;
    }

    private static async Task WaitForRunningState(
        DistributedApplication application,
        string serviceName,
        TimeSpan? timeout = null) 
        => await application.Services.GetRequiredService<ResourceNotificationService>()
            .WaitForResourceAsync(serviceName, KnownResourceStates.Running)
            .WaitAsync(timeout ?? TimeSpan.FromSeconds(30))
        ;

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        Application.Dispose();
    }
}