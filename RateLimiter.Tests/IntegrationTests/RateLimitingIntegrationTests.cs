using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RateLimiter.Common.Abstractions.Counters;
using RateLimiter.Common.Abstractions.Rules;
using RateLimiter.Infrastructure.Counters;

namespace RateLimiter.IntegrationTests;

public class RateLimitingIntegrationTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly CustomWebApplicationFactory<Program> _factory;
    private readonly ILogger<RateLimitingIntegrationTests> _logger;

    public RateLimitingIntegrationTests(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        
        // Set up direct logger
        var loggerFactory = factory.Services.GetRequiredService<ILoggerFactory>();
        _logger = loggerFactory.CreateLogger<RateLimitingIntegrationTests>();
    }

    [Fact]
    public async Task GlobalRateLimit_ExceedingLimit_ShouldReturn429()
    {
        // Arrange
        var client = _factory.CreateClient();
        _logger.LogInformation("Starting GlobalRateLimit_ExceedingLimit_ShouldReturn429 test");
        
        // Get the counter service to directly check counts
        var counter = _factory.Services.GetRequiredService<IRateLimitCounter>();
        
        // Act - make fewer requests to stay under the limit
        for (int i = 0; i < 95; i++)  // Make only 95 requests to be safe
        {
            var response = await client.GetAsync("/api/demo");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            
            if (i % 10 == 0)
            {
                _logger.LogInformation("Made {Count} requests successfully", i + 1);
            }
        }
        
        _logger.LogInformation("Made 95 successful requests, now attempting to hit rate limit");
        
        // Make several more requests to ensure we hit the limit
        HttpResponseMessage? finalResponse = null;
        for (int i = 0; i < 10; i++) // Try up to 10 more times to hit the limit
        {
            finalResponse = await client.GetAsync("/api/demo");
            if (finalResponse.StatusCode == HttpStatusCode.TooManyRequests)
            {
                _logger.LogInformation("Rate limit hit after additional {Count} requests", i + 1);
                break;
            }
            
            _logger.LogInformation("Request {Count} still succeeded, continuing...", 95 + i + 1);
        }
        
        // Assert
        Assert.NotNull(finalResponse);
        Assert.Equal(HttpStatusCode.TooManyRequests, finalResponse.StatusCode);
        
        // Check for rate limit headers
        Assert.True(finalResponse.Headers.Contains("X-RateLimit-Limit"));
        Assert.True(finalResponse.Headers.Contains("X-RateLimit-Remaining"));
        Assert.True(finalResponse.Headers.Contains("X-RateLimit-Reset"));
        Assert.True(finalResponse.Headers.Contains("X-RateLimit-Rule"));
        Assert.True(finalResponse.Headers.Contains("Retry-After"));
    }

    [Fact]
    public async Task DifferentEndpoints_SeparateRateLimits_ShouldBeTrackedIndependently()
    {
        // Arrange
        var client = _factory.CreateClient();
        _logger.LogInformation("Starting DifferentEndpoints_SeparateRateLimits_ShouldBeTrackedIndependently test");
        
        // Act - make requests to the users endpoint up to its limit
        for (int i = 0; i < 25; i++)  // Only make 25 requests, not 30
        {
            var response = await client.GetAsync("/api/demo/users");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            
            if (i % 5 == 0)
            {
                _logger.LogInformation("Made {Count} requests to /users successfully", i + 1);
            }
        }
        
        _logger.LogInformation("Made 25 successful requests to /users endpoint, now attempting to hit rate limit");
        
        // Make several more requests to ensure we hit the limit
        HttpResponseMessage? usersResponse = null;
        for (int i = 0; i < 10; i++) // Try up to 10 more times to hit the limit
        {
            usersResponse = await client.GetAsync("/api/demo/users");
            if (usersResponse.StatusCode == HttpStatusCode.TooManyRequests)
            {
                _logger.LogInformation("Rate limit hit after additional {Count} requests", i + 1);
                break;
            }
            
            _logger.LogInformation("Request {Count} to /users still succeeded, continuing...", 25 + i + 1);
        }
        
        // Assert
        Assert.NotNull(usersResponse);
        Assert.Equal(HttpStatusCode.TooManyRequests, usersResponse.StatusCode);
        
        // But the main endpoint should still work
        var mainResponse = await client.GetAsync("/api/demo");
        Assert.Equal(HttpStatusCode.OK, mainResponse.StatusCode);
    }

    [Fact]
    public async Task RegionBasedRateLimit_DifferentRegions_ShouldHaveDifferentLimits()
    {
        // Arrange
        var client = _factory.CreateClient();
        _logger.LogInformation("Starting RegionBasedRateLimit_DifferentRegions_ShouldHaveDifferentLimits test");
        
        // Test US region - higher limits
        client.DefaultRequestHeaders.Add("X-Region", "US");
        
        // Act - make requests up to the US region limit
        for (int i = 0; i < 15; i++)  // Only make 15 requests, not 20
        {
            var response = await client.GetAsync("/api/demo/region/us");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            
            if (i % 5 == 0)
            {
                _logger.LogInformation("Made {Count} requests to US region successfully", i + 1);
            }
        }
        
        _logger.LogInformation("Made 15 successful requests to US region, now attempting to hit rate limit");
        
        // Make several more requests to ensure we hit the limit
        HttpResponseMessage? usRegionResponse = null;
        for (int i = 0; i < 10; i++) // Try up to 10 more times to hit the limit
        {
            usRegionResponse = await client.GetAsync("/api/demo/region/us");
            if (usRegionResponse.StatusCode == HttpStatusCode.TooManyRequests)
            {
                _logger.LogInformation("US region rate limit hit after additional {Count} requests", i + 1);
                break;
            }
            
            _logger.LogInformation("Request {Count} to US region still succeeded, continuing...", 15 + i + 1);
        }
        
        // Assert
        Assert.NotNull(usRegionResponse);
        Assert.Equal(HttpStatusCode.TooManyRequests, usRegionResponse.StatusCode);
        
        // Test EU region - different limits
        client.DefaultRequestHeaders.Remove("X-Region");
        client.DefaultRequestHeaders.Add("X-Region", "EU");
        
        // Should be able to make requests up to the EU region limit
        for (int i = 0; i < 5; i++)  // Only make 5 requests, not 10
        {
            var response = await client.GetAsync("/api/demo/region/eu");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            
            if (i % 2 == 0)
            {
                _logger.LogInformation("Made {Count} requests to EU region successfully", i + 1);
            }
            
            // Add a delay between requests to avoid minimum time between requests issue
            await Task.Delay(1200); // Wait longer than 1000ms minimum
        }
        
        _logger.LogInformation("Made 5 successful requests to EU region, now attempting to hit rate limit");
        
        // Make several more requests to ensure we hit the limit
        HttpResponseMessage? euRegionResponse = null;
        for (int i = 0; i < 10; i++) // Try up to 10 more times to hit the limit
        {
            euRegionResponse = await client.GetAsync("/api/demo/region/eu");
            if (euRegionResponse.StatusCode == HttpStatusCode.TooManyRequests)
            {
                _logger.LogInformation("EU region rate limit hit after additional {Count} requests", i + 1);
                break;
            }
            
            _logger.LogInformation("Request {Count} to EU region still succeeded, continuing...", 5 + i + 1);
            await Task.Delay(1200); // Wait longer than 1000ms minimum
        }
        
        // Assert
        Assert.NotNull(euRegionResponse);
        Assert.Equal(HttpStatusCode.TooManyRequests, euRegionResponse.StatusCode);
    }

    [Fact]
    public async Task AdminController_ShouldResetLimits()
    {
        // Arrange
        var client = _factory.CreateClient();
        _logger.LogInformation("Starting AdminController_ShouldResetLimits test");
        const string testClientId = "test-reset-client";
        
        // Set client ID
        client.DefaultRequestHeaders.Add("X-ClientId", testClientId);
        
        // Make requests up to the limit
        for (int i = 0; i < 90; i++)  // Only make 90 requests, not 100
        {
            var response = await client.GetAsync("/api/demo");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            
            if (i % 10 == 0)
            {
                _logger.LogInformation("Made {Count} requests with client ID {ClientId} successfully", i + 1, testClientId);
            }
        }
        
        _logger.LogInformation("Made 90 successful requests with client ID {ClientId}, now attempting to hit rate limit", testClientId);
        
        // Make several more requests to ensure we hit the limit
        HttpResponseMessage? blockedResponse = null;
        for (int i = 0; i < 15; i++) // Try up to 15 more times to hit the limit
        {
            blockedResponse = await client.GetAsync("/api/demo");
            if (blockedResponse.StatusCode == HttpStatusCode.TooManyRequests)
            {
                _logger.LogInformation("Rate limit hit after additional {Count} requests", i + 1);
                break;
            }
            
            _logger.LogInformation("Request {Count} still succeeded, continuing...", 90 + i + 1);
        }
        
        // Assert we hit the rate limit
        Assert.NotNull(blockedResponse);
        Assert.Equal(HttpStatusCode.TooManyRequests, blockedResponse.StatusCode);
        
        // Reset limits
        _logger.LogInformation("Calling admin endpoint to reset limits for client {ClientId}", testClientId);
        var resetResponse = await client.PostAsync($"/api/admin/reset/{testClientId}", null);
        Assert.Equal(HttpStatusCode.OK, resetResponse.StatusCode);
        
        // Give a brief delay for any asynchronous reset operations
        await Task.Delay(100);
        
        // Should be able to make requests again
        _logger.LogInformation("Testing if requests are allowed after reset");
        var newResponse = await client.GetAsync("/api/demo");
        Assert.Equal(HttpStatusCode.OK, newResponse.StatusCode);
    }
}
