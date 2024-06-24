using NUnit.Framework;
using System.Net.Http;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.TestHost;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;

namespace RateLimiter.Tests;

[TestFixture]
public class RateLimitingMiddlewareTests
{
    private TestServer _server;
    private HttpClient _client;

    [SetUp]
    public void SetUp()
    {
        var builder = new WebHostBuilder()
            .ConfigureServices(services =>
            {
                var rateLimiter = new RateLimiter();
                rateLimiter.AddDefaultRuleForResource("/api/resource1", new FixedWindowRateLimitRule(1, TimeSpan.FromMinutes(1)));
                rateLimiter.AddDefaultRuleForResource("/api/resource2", new TimeSinceLastCallRule(TimeSpan.FromSeconds(1)));
                rateLimiter.AddRuleForTokenAndResource("US", "/api/resource1", new FixedWindowRateLimitRule(2, TimeSpan.FromMinutes(1)));
                rateLimiter.AddRuleForTokenAndResource("EU", "/api/resource1", new TimeSinceLastCallRule(TimeSpan.FromMinutes(1)));

                services.AddSingleton(rateLimiter);
            })
            .Configure(app =>
            {
                app.UseMiddleware<RateLimitingMiddleware>();

                app.Run(async context =>
                {
                    await context.Response.WriteAsync("Test endpoint reached");
                });
            });

        _server = new TestServer(builder);
        _client = _server.CreateClient();
    }

    [Test]
    public async Task Should_Return429_When_RateLimitExceeded()
    {
        // Arrange
        var firstRequest = new HttpRequestMessage(HttpMethod.Get, "/api/resource1");
        firstRequest.Headers.Add("Authorization", "testToken");
        var secondRequest = new HttpRequestMessage(HttpMethod.Get, "/api/resource1");
        secondRequest.Headers.Add("Authorization", "testToken");

        // Act
        var firstResponse = await _client.SendAsync(firstRequest);
        var secondResponse = await _client.SendAsync(secondRequest);

        // Assert
        Assert.AreEqual(HttpStatusCode.OK, firstResponse.StatusCode, "First request should succeed.");
        Assert.AreEqual(HttpStatusCode.TooManyRequests, secondResponse.StatusCode, "Second request should be rate limited.");
    }

    [Test]
    public async Task Should_Return200_When_Not_Rate_Limited()
    {
        // Arrange
        var firstRequest = new HttpRequestMessage(HttpMethod.Get, "/api/not-limited-resource");
        firstRequest.Headers.Add("Authorization", "testToken");
        var secondRequest = new HttpRequestMessage(HttpMethod.Get, "/api/not-limited-resource");
        secondRequest.Headers.Add("Authorization", "testToken");

        // Act
        var firstResponse = await _client.SendAsync(firstRequest);
        var secondResponse = await _client.SendAsync(secondRequest);

        // Assert
        Assert.AreEqual(HttpStatusCode.OK, firstResponse.StatusCode, "First request should succeed.");
        Assert.AreEqual(HttpStatusCode.OK, secondResponse.StatusCode, "Second request should succeed.");
    }

    [Test]
    public async Task Should_Consider_Location()
    {
        // Arrange
        var firstRequest = new HttpRequestMessage(HttpMethod.Get, "/api/resource1");
        firstRequest.Headers.Add("Authorization", "testToken");
        firstRequest.Headers.Add("Location", "US");
        var secondRequest = new HttpRequestMessage(HttpMethod.Get, "/api/resource1");
        secondRequest.Headers.Add("Authorization", "testToken");
        secondRequest.Headers.Add("Location", "US");
        var thirdRequest = new HttpRequestMessage(HttpMethod.Get, "/api/resource1");
        thirdRequest.Headers.Add("Authorization", "testToken");
        thirdRequest.Headers.Add("Location", "US");
        var fourthRequest = new HttpRequestMessage(HttpMethod.Get, "/api/resource1");
        fourthRequest.Headers.Add("Authorization", "testToken");
        fourthRequest.Headers.Add("Location", "EU");
        var fifthRequest = new HttpRequestMessage(HttpMethod.Get, "/api/resource1");
        fifthRequest.Headers.Add("Authorization", "testToken");
        fifthRequest.Headers.Add("Location", "EU");

        // Act
        var firstResponse = await _client.SendAsync(firstRequest);
        var secondResponse = await _client.SendAsync(secondRequest);
        var thirdResponse = await _client.SendAsync(thirdRequest);
        var fourthResponse = await _client.SendAsync(fourthRequest);
        var fifthResponse = await _client.SendAsync(fifthRequest);

        // Assert
        Assert.AreEqual(HttpStatusCode.OK, firstResponse.StatusCode, "First request should succeed.");
        Assert.AreEqual(HttpStatusCode.OK, secondResponse.StatusCode, "Second request should succeed.");
        Assert.AreEqual(HttpStatusCode.TooManyRequests, thirdResponse.StatusCode, "Third request should be rate limited.");
        Assert.AreEqual(HttpStatusCode.OK, fourthResponse.StatusCode, "Fourth request should succeed.");
        Assert.AreEqual(HttpStatusCode.TooManyRequests, fifthResponse.StatusCode, "Fifth request should be rate limited.");
    }

    [Test]
    public async Task Should_429_When_No_Token()
    {
        // Arrange
        var firstRequest = new HttpRequestMessage(HttpMethod.Get, "/api/resource1");
        var secondRequest = new HttpRequestMessage(HttpMethod.Get, "/api/resource2");

        // Act
        var firstResponse = await _client.SendAsync(firstRequest);
        var secondResponse = await _client.SendAsync(secondRequest);

        // Assert
        Assert.AreEqual(HttpStatusCode.TooManyRequests, firstResponse.StatusCode, "First request should be rate limited.");
        Assert.AreEqual(HttpStatusCode.TooManyRequests, secondResponse.StatusCode, "Second request should be rate limited.");
    }

    [TearDown]
    public void TearDown()
    {
        _client.Dispose();
        _server.Dispose();
    }
}
