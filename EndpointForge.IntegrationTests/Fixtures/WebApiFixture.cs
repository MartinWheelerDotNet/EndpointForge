using Aspire.Hosting;
using Microsoft.Extensions.Hosting;

namespace EndpointForge.IntegrationTests.Fixtures;

public class WebApiFixture : IDisposable
{
    public readonly DistributedApplication Application;

    public WebApiFixture()
    { 
        var appHost = CreateDefaultAppHost().Result;
        Application = appHost.BuildAsync().Result;
        Application.Start();
        WaitForRunningState(Application, "webapi")
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