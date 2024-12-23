namespace EndpointForge.IntegrationTests;

public class ServiceStatusTests(WebApiTests webApiTests) : IClassFixture<WebApiTests>
{
    [Theory]
    [InlineData("webapi")]
    public async Task ResourceHealthEndpointReturnsHealthyWhenResourceIsRunning(string resourceName)
    {
        const string endpointName = "/health";
        using var httpClient = webApiTests.Application.CreateHttpClient(resourceName);
        
        var response = await httpClient.GetAsync(endpointName);
        var body = await response.Content.ReadAsStringAsync();
        
        Assert.Multiple(
            () => Assert.Equal(HttpStatusCode.OK, response.StatusCode),
            () => Assert.Equal("Healthy", body));
    }
}