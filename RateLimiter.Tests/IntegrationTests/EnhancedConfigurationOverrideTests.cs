using System.Net;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using RateLimiter.Core.Configuration;
using RateLimiter.Infrastructure.DependencyInjection;

namespace RateLimiter.IntegrationTests;

public class EnhancedConfigurationOverrideTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public EnhancedConfigurationOverrideTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Testing");
            builder.ConfigureServices(services =>
            {
                // Configure enhanced hybrid rate limiting with configuration override
                services.AddEnhancedHybridRateLimiting(
                    options =>
                    {
                        options.EnableRateLimiting = true;
                        options.IncludeHeaders = true;
                        options.HeaderPrefix = "X-RateLimit";
                        options.StatusCode = 429;
                        options.ClientIdHeaderName = "X-ClientId";
                        options.RegionHeaderName = "X-Region";
                    },
                    configRules =>
                    {
                        configRules.EnableConfigurationRules = true;
                        configRules.EnableAttributeRules = true;
                        configRules.ConflictResolutionStrategy = ConflictResolutionStrategy.ConfigurationWins;
                        configRules.LogConflicts = true;
                        
                        // Add configuration rule that should override the GlobalLimit attribute
                        configRules.Rules.Add(new RateLimitRuleConfiguration
                        {
                            Name = "BlogProtection",
                            Type = "FixedWindow",
                            MaxRequests = 15, // Higher than GlobalLimit (5)
                            TimeWindowSeconds = 60,
                            PathPattern = "/api/demo",
                            HttpMethods = "GET",
                            Enabled = true,
                            Priority = 10
                        });
                    });
            });
        });
    }

    [Fact]
    public async Task ConfigurationRule_ShouldOverride_AttributeRule()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act - Make requests beyond the original GlobalLimit (5) but within BlogProtection (15)
        var responses = new List<HttpResponseMessage>();
        for (int i = 0; i < 10; i++) // More than GlobalLimit (5) but less than BlogProtection (15)
        {
            var response = await client.GetAsync("/api/demo");
            responses.Add(response);
            
            // Small delay to avoid overwhelming
            await Task.Delay(50);
        }

        // Assert
        var successfulResponses = responses.Count(r => r.StatusCode == HttpStatusCode.OK);
        var blockedResponses = responses.Count(r => r.StatusCode == HttpStatusCode.TooManyRequests);

        // With configuration override working, we should get more than 5 successful requests
        Assert.True(successfulResponses > 5, 
            $"Expected more than 5 successful requests (configuration override), but got {successfulResponses}");
        
        // Check headers on first response
        var firstResponse = responses.First();
        Assert.True(firstResponse.Headers.Contains("X-RateLimit-Rule"));
        
        var ruleHeader = firstResponse.Headers.GetValues("X-RateLimit-Rule").FirstOrDefault();
        // Should show configuration rule name, not GlobalLimit
        Assert.NotEqual("GlobalLimit", ruleHeader);
    }

    [Fact]
    public async Task ConfigurationRule_HeadersShould_ReflectConfigurationRule()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/demo");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        // Verify configuration rule is active
        Assert.True(response.Headers.Contains("X-RateLimit-Limit"));
        Assert.True(response.Headers.Contains("X-RateLimit-Rule"));
        
        var limitHeader = response.Headers.GetValues("X-RateLimit-Limit").FirstOrDefault();
        var ruleHeader = response.Headers.GetValues("X-RateLimit-Rule").FirstOrDefault();
        
        // Should show higher limit from configuration (15) not attribute (5)
        Assert.Equal("15", limitHeader);
        
        // Should show configuration rule name
        Assert.Equal("BlogProtection", ruleHeader);
    }
}
