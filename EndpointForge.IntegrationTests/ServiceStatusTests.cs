using EndpointForge.IntegrationTests.Fixtures;
using FluentAssertions;

namespace EndpointForge.IntegrationTests;

public class ServiceStatusTests(WebApiFixture webApiFixture) : IClassFixture<WebApiFixture>
{
    [Theory]
    [InlineData("webapi")]
    public async Task ResourceHealthEndpointReturnsHealthyWhenResourceIsRunning(string resourceName)
    {
        const string endpointName = "/health";
        using var httpClient = webApiFixture.Application.CreateHttpClient(resourceName);
        
        var response = await httpClient.GetAsync(endpointName);
        var body = await response.Content.ReadAsStringAsync();
        
        Assert.Multiple(
            () => response.StatusCode.Should().Be(HttpStatusCode.OK),
            () => body.Should().Be("Healthy"));
    }
}