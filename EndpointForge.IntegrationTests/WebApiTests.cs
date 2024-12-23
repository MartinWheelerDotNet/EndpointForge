using Aspire.Hosting;
using Microsoft.Extensions.Hosting;

namespace EndpointForge.IntegrationTests;

public class WebApiTests : IDisposable
{
    public readonly DistributedApplication Application;
    
    public WebApiTests()
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
    
    protected static async Task WaitForRunningState(
        DistributedApplication application,
        string serviceName,
        TimeSpan? timeout = null) 
        => await application.Services.GetRequiredService<ResourceNotificationService>()
            .WaitForResourceAsync(serviceName, KnownResourceStates.Running)
            .WaitAsync(timeout ?? TimeSpan.FromSeconds(30))
        ;

    public void Dispose()
    {
        Application.Dispose();
    }
}