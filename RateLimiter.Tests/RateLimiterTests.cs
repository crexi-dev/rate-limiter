using Shouldly;
using Xunit.Abstractions;

namespace RateLimiter.Tests;

public class RateLimiterTests
{
    [Fact]
    public async Task SimpleRuleTest()
    {
        var rule = new RateLimiterRule(TimeSpan.FromSeconds(2), 1);

        var rateLimiter = new RateLimiter(new RateLimiterStorage(), new List<RateLimiterRule> { rule });

        var rate = 0;
        
        for (var i = 0; i < 10; i++)
        {
            var now = DateTime.UtcNow;
            var attempt = new Attempt(now);
            var succeed = await rateLimiter.Try("key", attempt);
            
            if (succeed)
            {
                rate++;
            }
            
            Thread.Sleep(1000);
        }

        rate.ShouldBe(5);
    }

    [Fact]
    public async Task RegionalRulesTest()
    {
        var euRule = new RateLimiterRule(TimeSpan.FromSeconds(60), 30, new []{ new RateLimiterParameter("region", "eu") });
        var usRule = new RateLimiterRule(TimeSpan.FromSeconds(60), 10, new [] { new RateLimiterParameter("region", "us")});

        var rateLimiter = new RateLimiter(new RateLimiterStorage(), new List<RateLimiterRule> { euRule, usRule });

        var euRate = 0;
        var usRate = 0;
        
        var now = DateTime.UtcNow.Ticks;
        
        for (var i = 0; i < 60; i++)
        {
            var euAttempt = new Attempt(now, new []{ new RateLimiterParameter("region", "eu") });
            var usAttempt = new Attempt(now, new []{ new RateLimiterParameter("region", "us") });
            
            var euResult = await rateLimiter.Try("key", euAttempt);
            var usResult = await rateLimiter.Try("key", usAttempt);

            if (euResult)
            {
                euRate++;
            }

            if (usResult)
            {
                usRate++;
            }

            now += TimeSpan.FromSeconds(1).Ticks;
        }

        usRate.ShouldBe(10);
        euRate.ShouldBe(30);
    }
    
    [Fact]
    public async Task MixedRulesTest()
    {
        var rule = new RateLimiterRule(TimeSpan.FromSeconds(60), 20);
        var euRule = new RateLimiterRule(TimeSpan.FromSeconds(60), 30, new []{ new RateLimiterParameter("region", "eu") });
        var usRule = new RateLimiterRule(TimeSpan.FromSeconds(60), 10, new [] { new RateLimiterParameter("region", "us")});

        var rateLimiter = new RateLimiter(new RateLimiterStorage(), new List<RateLimiterRule> { rule, euRule, usRule });

        var euRate = 0;
        var usRate = 0;
        
        var now = DateTime.UtcNow.Ticks;
        
        for (var i = 0; i < 60; i++)
        {
            var euAttempt = new Attempt(now, new []{ new RateLimiterParameter("region", "eu") });
            var usAttempt = new Attempt(now, new []{ new RateLimiterParameter("region", "us") });
            
            var euResult = await rateLimiter.Try("key", euAttempt);
            var usResult = await rateLimiter.Try("key", usAttempt);

            if (euResult)
            {
                euRate++;
            }

            if (usResult)
            {
                usRate++;
            }

            now += TimeSpan.FromSeconds(1).Ticks;
        }

        usRate.ShouldBe(10);
        euRate.ShouldBe(10);
    }
}