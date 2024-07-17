using RateLimiter.RateLimitingRules;
using Microsoft.Extensions.Time.Testing;

namespace RateLimiter.Tests;

[TestClass]
public class RateLimiterWithRulesTests
{
    [TestMethod]
    public void RequestsPerTimeTests()
    {
        TimeProvider timeProvider = new FakeTimeProvider(DateTimeOffset.UtcNow);
        RateLimiter<string, string> rateLimiter = new(new([(x => true, x => [new RequestsPerTimeRule<string, string>(timeProvider, 3, TimeSpan.FromMinutes(1))])]));

        rateLimiter.RegisterRequest("", "");
        rateLimiter.RegisterRequest("", "");
        bool hasReachedLimit = rateLimiter.HasExceededLimit("", "");

        Assert.IsFalse(hasReachedLimit);
    }

    [TestMethod]
    public void RequestsPerTimeMultipleClientsTests()
    {
        TimeProvider timeProvider = new FakeTimeProvider(DateTimeOffset.UtcNow);
        RateLimiter<string, string> rateLimiter = new(new([(x => true, x => [new RequestsPerTimeRule<string, string>(timeProvider, 3, TimeSpan.FromMinutes(1))])]));

        rateLimiter.RegisterRequest("1", "");
        rateLimiter.RegisterRequest("1", "");
        bool hasReachedLimit1 = rateLimiter.HasExceededLimit("1", "");

        rateLimiter.RegisterRequest("2", "");
        rateLimiter.RegisterRequest("2", "");
        bool hasReachedLimit2 = rateLimiter.HasExceededLimit("2", "");

        Assert.IsFalse(hasReachedLimit1);
        Assert.IsFalse(hasReachedLimit2);
    }

    [TestMethod]
    public void RequestsPerTimeMultipleClientsSharedTests()
    {
        TimeProvider timeProvider = new FakeTimeProvider(DateTimeOffset.UtcNow);
        var rule = new RequestsPerTimeRule<string, string>(timeProvider, 3, TimeSpan.FromMinutes(1));
        RateLimiter<string, string> rateLimiter = new(new([(x => true, x => [rule])]));

        rateLimiter.RegisterRequest("1", "");
        rateLimiter.RegisterRequest("1", "");
        bool hasReachedLimit1 = rateLimiter.HasExceededLimit("1", "");

        rateLimiter.RegisterRequest("2", "");
        rateLimiter.RegisterRequest("2", "");
        bool hasReachedLimit2 = rateLimiter.HasExceededLimit("2", "");

        Assert.IsFalse(hasReachedLimit1);
        Assert.IsTrue(hasReachedLimit2);
    }

    [TestMethod]
    public void TimeSinceRequestTests()
    {
        FakeTimeProvider timeProvider = new FakeTimeProvider(DateTimeOffset.UtcNow);
        RateLimiter<string, string> rateLimiter = new(new([(x => true, x => [new TimeSinceLastRequestRule<string, string>(timeProvider, TimeSpan.FromMinutes(1))])]));

        bool hasReachedLimit = rateLimiter.HasExceededLimit("", "");
        rateLimiter.RegisterRequest("", "");
        bool hasReachedLimit2 = rateLimiter.HasExceededLimit("", "");
        timeProvider.Advance(TimeSpan.FromMinutes(1));
        bool hasReachedLimit3 = rateLimiter.HasExceededLimit("", "");

        Assert.IsFalse(hasReachedLimit);
        Assert.IsTrue(hasReachedLimit2);
        Assert.IsFalse(hasReachedLimit3);
    }
}
