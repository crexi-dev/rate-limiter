using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using RateLimiter.Attributes;
using RateLimiter.Rules.Interfaces;

namespace RateLimiter.Tests;

[TestFixture]
public class RateLimiterMiddlewareTests
{
    private Mock<RequestDelegate> mockRequestDelegate;
    private Mock<IOptions<RateLimitOptions>> mockOptions;
    private Mock<IRateLimiterRule> mockRateLimiterRule;
    private HttpContext httpContext;
    private string token = "testToken";

    [SetUp]
    public void Setup()
    {
        mockRequestDelegate = new Mock<RequestDelegate>();
        mockOptions = new Mock<IOptions<RateLimitOptions>>();
        mockRateLimiterRule = new Mock<IRateLimiterRule>();

        httpContext = new DefaultHttpContext();
        httpContext.Request.Headers["x-api-token"] = token;
    }

    [Test]
    public async Task InvokeAsync_WhenApiTokenIsMissing_ThenForbidden()
    {
        var middleware = new RateLimiterMiddleware(
            mockRequestDelegate.Object,
            mockOptions.Object,
            new[] { mockRateLimiterRule.Object });
        httpContext.Request.Headers.Remove("x-api-token");

        await middleware.InvokeAsync(httpContext);

        Assert.AreEqual((int)HttpStatusCode.Forbidden, httpContext.Response.StatusCode);
    }

    [Test]
    public async Task InvokeAsync_WhenRuleHasNotBeenPassed_ThenTooManyRequests()
    {
        var ruleName = "RuleName";
        var rateRulesAttribute = new RateRulesAttribute(ruleName);
        var endpoint = new Endpoint(_ => Task.CompletedTask,
            new EndpointMetadataCollection(rateRulesAttribute), "Test");
        httpContext.SetEndpoint(endpoint);
        mockRateLimiterRule = new Mock<IRateLimiterRule>();
        mockRateLimiterRule.Setup(r => r.Name).Returns(ruleName);
        mockRateLimiterRule.Setup(r => r.IsAllowed(token, It.IsAny<RateLimitOptions>()))
            .Returns(Task.FromResult(false));

        var middleware = new RateLimiterMiddleware(
            mockRequestDelegate.Object,
            mockOptions.Object,
            new[] { mockRateLimiterRule.Object });

        await middleware.InvokeAsync(httpContext);

        Assert.AreEqual((int)HttpStatusCode.TooManyRequests, httpContext.Response.StatusCode);
    }

    [Test]
    public async Task InvokeAsync_WhenRuleHasBeenPassed_ThenCallsNextDelegate()
    {
        bool nextDelegateWasCalled = false;
        var ruleName = "RuleName";
        var rateRulesAttribute = new RateRulesAttribute(ruleName);
        var endpoint = new Endpoint(_ => Task.CompletedTask,
            new EndpointMetadataCollection(rateRulesAttribute), "Test");
        httpContext.SetEndpoint(endpoint);

        mockRateLimiterRule = new Mock<IRateLimiterRule>();
        mockRateLimiterRule.Setup(r => r.Name).Returns(ruleName);
        mockRateLimiterRule.Setup(r => r.IsAllowed(token, It.IsAny<RateLimitOptions>()))
            .Returns(Task.FromResult(true));

        var middleware = new RateLimiterMiddleware(
            _ =>
            {
                nextDelegateWasCalled = true;
                return Task.CompletedTask;
            },
            mockOptions.Object,
            new[] { mockRateLimiterRule.Object });

        await middleware.InvokeAsync(httpContext);

        Assert.IsTrue(nextDelegateWasCalled);
    }
}