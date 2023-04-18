using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RateLimiter.Abstractions;
using RateLimiter.Models.RateLimits;

namespace RateLimiter.Fakes;

public class FakeRateLimitService : IRateLimitService
{
    public Task<IEnumerable<IRateLimit>> GetRateLimitsAsync(string apiKey)
    {
        return Task.FromResult(FakeRateLimitsData.TryGetValue(apiKey, out var rateLimits)
            ? rateLimits
            : Enumerable.Empty<IRateLimit>());
    }

    private static Dictionary<string, IRateLimit[]> FakeRateLimitsData => new()
    {
        {
            "too-many-requests-in-month", new IRateLimit[]
            {
                new RequestsPerLastTimeRateLimit
                {
                    TimeSpan = TimeSpan.FromDays(30),
                    MaxCallCount = 2
                }
            }
        },
        {
            "not-enough-wait", new IRateLimit[]
            {
                new TimeFromLastCallRateLimit
                {
                    TimeFromLastCall = TimeSpan.FromSeconds(1)
                }
            }
        },
        {
            "no-restrictions", new IRateLimit[]
            {
                new TimeFromLastCallRateLimit
                {
                    TimeFromLastCall = TimeSpan.FromSeconds(30)
                },
                new RequestsPerLastTimeRateLimit
                {
                    TimeSpan = TimeSpan.FromMinutes(30),
                    MaxCallCount = 2
                }
            }
        }
    };
}