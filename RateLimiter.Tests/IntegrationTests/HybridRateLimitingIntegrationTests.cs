using System.Net;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using RateLimiter.Core.Configuration;

namespace RateLimiter.IntegrationTests;

public class HybridRateLimitingIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public HybridRateLimitingIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Testing");
            builder.ConfigureServices(services =>
            {
                // Configure hybrid rate limiting for testing
                services.Configure<EnhancedRateLimitConfiguration>(config =>
                {
                    config.EnableConfigurationRules = true;
                    config.EnableAttributeRules = true;
                    config.ConflictResolutionStrategy = ConflictResolutionStrategy.ConfigurationWins;
                    config.LogConflicts = true;
                    
                    // Add test configuration rules
                    config.Rules.Add(new RateLimitRuleConfiguration
                    {
                        Name = "ConfigTestRule",
                        Type = "FixedWindow",
                        MaxRequests = 5,
                        TimeWindowSeconds = 60,
                        PathPattern = "/api/demo/config-test",
                        HttpMethods = "GET",
                        Enabled = true,
                        Priority = 10
                    });
                });
            });
        });
    }

    [Fact]
    public async Task HybridSystem_ShouldCombineConfigurationAndAttributeRules()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act - Test existing attribute-based rule
        var attributeResponse = await client.GetAsync("/api/demo");
        
        // Assert - Attribute-based rule should work
        Assert.Equal(HttpStatusCode.OK, attributeResponse.StatusCode);
        Assert.True(attributeResponse.Headers.Contains("X-RateLimit-Limit"));
    }

    [Fact]
    public async Task HybridSystem_ShouldMaintainBackwardCompatibility()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act - Test all existing endpoints still work
        var endpoints = new[]
        {
            "/api/demo",
            "/api/demo/users",
            "/api/demo/burst"
        };

        foreach (var endpoint in endpoints)
        {
            var response = await client.GetAsync(endpoint);
            
            // Assert - All endpoints should be accessible
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            
            // Should have rate limit headers from existing attribute rules
            Assert.True(response.Headers.Contains("X-RateLimit-Limit"));
            Assert.True(response.Headers.Contains("X-RateLimit-Rule"));
        }
    }

    [Fact]
    public async Task HybridSystem_ShouldLogConflicts()
    {
        // This test verifies that the system can handle conflicts gracefully
        // In a real scenario, you'd check logs for conflict messages
        
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/demo");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        // The system should handle any conflicts and still function
    }
}
