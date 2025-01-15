using EndpointForge.IntegrationTests.Fixtures;
using FluentAssertions;

namespace EndpointForge.IntegrationTests.StatusTests;

public class ServiceStatusTests(EndpointForgeFixture endpointForgeFixture) : IClassFixture<EndpointForgeFixture>
{
    [Theory]
    [InlineData("EndpointForge")]
    public async Task ResourceHealthEndpointReturnsHealthyWhenResourceIsRunning(string resourceName)
    {
        const string endpointName = "/health";
        using var httpClient = endpointForgeFixture.Application.CreateHttpClient(resourceName);
        
        var response = await httpClient.GetAsync(endpointName);
        var body = await response.Content.ReadAsStringAsync();
        
        Assert.Multiple(
            () => response.StatusCode.Should().Be(HttpStatusCode.OK),
            () => body.Should().Be("Healthy"));
    }
}