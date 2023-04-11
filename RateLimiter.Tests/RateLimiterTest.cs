using NUnit.Framework;
using System;

namespace RateLimiter.Tests;

[TestFixture]
public class RateLimiterTest
{
    [Test]
    public void TenRequestsPerTenSeconds()
    {
        string token = Guid.NewGuid().ToString();
        var rateLimiter = new RateLimiter<string>(t => new RateLimiterQueue()
            .EnableRateLimit(10, new TimeSpan(0, 0, 10)));
        Assert.True(rateLimiter.Check(token, new DateTime(2023, 4, 11, 0, 0, 0, 0)));
        Assert.True(rateLimiter.Check(token, new DateTime(2023, 4, 11, 0, 0, 0, 1)));
        Assert.True(rateLimiter.Check(token, new DateTime(2023, 4, 11, 0, 0, 0, 2)));
        Assert.True(rateLimiter.Check(token, new DateTime(2023, 4, 11, 0, 0, 0, 3)));
        Assert.True(rateLimiter.Check(token, new DateTime(2023, 4, 11, 0, 0, 0, 4)));
        Assert.True(rateLimiter.Check(token, new DateTime(2023, 4, 11, 0, 0, 0, 5)));
        Assert.True(rateLimiter.Check(token, new DateTime(2023, 4, 11, 0, 0, 0, 6)));
        Assert.True(rateLimiter.Check(token, new DateTime(2023, 4, 11, 0, 0, 0, 7)));
        Assert.True(rateLimiter.Check(token, new DateTime(2023, 4, 11, 0, 0, 0, 8)));
        Assert.True(rateLimiter.Check(token, new DateTime(2023, 4, 11, 0, 0, 0, 9)));
        Assert.False(rateLimiter.Check(token, new DateTime(2023, 4, 11, 0, 0, 0, 10)));
        Assert.True(rateLimiter.Check(token, new DateTime(2023, 4, 11, 0, 0, 10, 1)));
        Assert.True(rateLimiter.Check(token, new DateTime(2023, 4, 11, 0, 0, 10, 2)));
    }

    [Test]
    public void OneSecondPeriod()
    {
        string token = Guid.NewGuid().ToString();
        var rateLimiter = new RateLimiter<string>(t => new RateLimiterQueue()
            .EnablePeriodLimit(new TimeSpan(0, 0, 1)));
        Assert.True(rateLimiter.Check(token, new DateTime(2023, 4, 11, 0, 0, 0, 0)));
        Assert.False(rateLimiter.Check(token, new DateTime(2023, 4, 11, 0, 0, 0, 1)));
        Assert.False(rateLimiter.Check(token, new DateTime(2023, 4, 11, 0, 0, 0, 2)));
        Assert.True(rateLimiter.Check(token, new DateTime(2023, 4, 11, 0, 0, 1, 3)));
        Assert.False(rateLimiter.Check(token, new DateTime(2023, 4, 11, 0, 0, 1, 4)));
    }

    [Test]
    public void TenRequestPerTenSecondsWithOneSecondPeriod()
    {
        string token = Guid.NewGuid().ToString();
        var rateLimiter = new RateLimiter<string>(t => new RateLimiterQueue()
            .EnableRateLimit(5, new TimeSpan(0, 0, 10))
            .EnablePeriodLimit(new TimeSpan(0, 0, 1)));
        Assert.True(rateLimiter.Check(token, new DateTime(2023, 4, 11, 0, 0, 0, 0)));
        Assert.False(rateLimiter.Check(token, new DateTime(2023, 4, 11, 0, 0, 0, 1)));
        Assert.False(rateLimiter.Check(token, new DateTime(2023, 4, 11, 0, 0, 0, 2)));
        Assert.True(rateLimiter.Check(token, new DateTime(2023, 4, 11, 0, 0, 1, 0)));
        Assert.True(rateLimiter.Check(token, new DateTime(2023, 4, 11, 0, 0, 2, 0)));
        Assert.True(rateLimiter.Check(token, new DateTime(2023, 4, 11, 0, 0, 3, 0)));
        Assert.False(rateLimiter.Check(token, new DateTime(2023, 4, 11, 0, 0, 3, 1)));
        Assert.True(rateLimiter.Check(token, new DateTime(2023, 4, 11, 0, 0, 4, 0)));
        Assert.False(rateLimiter.Check(token, new DateTime(2023, 4, 11, 0, 0, 5, 0)));
        Assert.False(rateLimiter.Check(token, new DateTime(2023, 4, 11, 0, 0, 6, 0)));
        Assert.False(rateLimiter.Check(token, new DateTime(2023, 4, 11, 0, 0, 7, 0)));
        Assert.False(rateLimiter.Check(token, new DateTime(2023, 4, 11, 0, 0, 9, 0)));
        Assert.True(rateLimiter.Check(token, new DateTime(2023, 4, 11, 0, 0, 10, 0)));
        Assert.True(rateLimiter.Check(token, new DateTime(2023, 4, 11, 0, 0, 11, 0)));
    }

    [Test]
    public void TwoTokens()
    {
        string usToken = $"US-{Guid.NewGuid()}";
        string euToken = $"EU-{Guid.NewGuid()}";
        var rateLimiter = new RateLimiter<string>(t =>
        {
            if (t.StartsWith("US"))
            {
                return new RateLimiterQueue()
                    .EnableRateLimit(5, new TimeSpan(0, 0, 10));
            }
            if (t.StartsWith("EU"))
            {
                return new RateLimiterQueue()
                    .EnablePeriodLimit(new TimeSpan(0, 0, 1));
            }
            throw new NotImplementedException();
        });
        Assert.True(rateLimiter.Check(usToken, new DateTime(2023, 4, 11, 0, 0, 0, 0)));
        Assert.True(rateLimiter.Check(euToken, new DateTime(2023, 4, 11, 0, 0, 0, 1)));
        Assert.True(rateLimiter.Check(usToken, new DateTime(2023, 4, 11, 0, 0, 0, 2)));
        Assert.True(rateLimiter.Check(usToken, new DateTime(2023, 4, 11, 0, 0, 0, 3)));
        Assert.False(rateLimiter.Check(euToken, new DateTime(2023, 4, 11, 0, 0, 0, 4)));
        Assert.True(rateLimiter.Check(usToken, new DateTime(2023, 4, 11, 0, 0, 0, 5)));
        Assert.True(rateLimiter.Check(usToken, new DateTime(2023, 4, 11, 0, 0, 0, 6)));
        Assert.False(rateLimiter.Check(usToken, new DateTime(2023, 4, 11, 0, 0, 0, 7)));
        Assert.True(rateLimiter.Check(euToken, new DateTime(2023, 4, 11, 0, 0, 1, 1)));
        Assert.True(rateLimiter.Check(usToken, new DateTime(2023, 4, 11, 0, 0, 10, 0)));
    }
}
