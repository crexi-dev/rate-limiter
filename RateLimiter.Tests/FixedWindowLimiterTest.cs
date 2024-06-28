using NUnit.Framework;
using RateLimiter.RateLimiter;
using RateLimiter.RateLimiter.Models;
using RateLimiter.RateLimiter.Options;
using System;

namespace RateLimiter.Tests;

[TestFixture]
public class FixedWindowLimiterTest
{
    private FixedWindowLimiter _subject;
    private FixedWindowLimiterOptions _options;

    [SetUp]
    public void Setup()
    {
        _options = new FixedWindowLimiterOptions
        {
            Limit = 2,
            Window = TimeSpan.FromSeconds(3),
        };

        _subject = new FixedWindowLimiter(_options);
    }

    [Test]
    public void CheckLimit_LimitIsNotReached_ShouldAllowRequest()
    {
        var clientRequest = new ClientRequest
        {
            LastHitAt = DateTime.MinValue.ToUniversalTime(),
            AmountOfHits = 0,
        };

        var result = _subject.CheckLimit(clientRequest);

        Assert.That(result.Limited, Is.False);
        Assert.That(result.CurrentLimit, Is.EqualTo(_options.Limit));
        Assert.That(result.RemainingAmountOfCalls, Is.EqualTo(1));
    }

    [Test]
    public void CheckLimit_LimitIsReached_ShouldNotAllowRequest()
    {
        var clientRequest = new ClientRequest
        {
            LastHitAt = DateTime.UtcNow,
            AmountOfHits = 2,
        };

        var result = _subject.CheckLimit(clientRequest);

        Assert.That(result.Limited, Is.True);
        Assert.That(result.CurrentLimit, Is.EqualTo(_options.Limit));
        Assert.That(result.RemainingAmountOfCalls, Is.EqualTo(0));
    }

    [Test]
    public void CheckLimit_TimeWindowIsExpired_ShouldAllowRequest()
    {
        var clientRequest = new ClientRequest
        {
            LastHitAt = DateTime.UtcNow.AddSeconds(-10),
            AmountOfHits = 2,
        };

        var result = _subject.CheckLimit(clientRequest);

        Assert.That(result.Limited, Is.False);
        Assert.That(result.CurrentLimit, Is.EqualTo(_options.Limit));
        Assert.That(result.RemainingAmountOfCalls, Is.EqualTo(1));
    }
}