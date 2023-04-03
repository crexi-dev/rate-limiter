using NUnit.Framework;
using RateLimiter.Algorithms.PredefinedAlgorithms;
using RateLimiter.Exceptions;
using System;
using System.Threading;

namespace RateLimiter.Tests;

[TestFixture]
public class RateLimiterTest
{
	[Test]
	public void ShouldFallIfLimitExceeded()
	{
		var limiter = RateLimiter.RateLimiter.Create(new SimpleLimiterAlgorithm(10, TimeSpan.FromSeconds(1)));

		Assert.Throws<LimitExceededException>(() =>
		{
			for (var i = 0; i < 11; i++)
			{
				limiter.PassOrThrough("test", "test");
			}
		});
    }

    [Test]
    public void ShouldResetLimiterInTimeout()
    {
        var limiter = RateLimiter.RateLimiter.Create(new SimpleLimiterAlgorithm(10, TimeSpan.FromSeconds(1)));

        Assert.DoesNotThrow(() =>
        {
            for (var i = 0; i < 10; i++)
            {
                limiter.PassOrThrough("test", "test");
            }
            Thread.Sleep(1500);
            for (var i = 0; i < 10; i++)
            {
                limiter.PassOrThrough("test", "test");
            }
        });
    }
}