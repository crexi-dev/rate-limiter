using System;
using System.Linq;
using RateLimiter.Abstractions;
using RateLimiter.Models;
using RateLimiter.Models.RateLimits;

namespace RateLimiter;

public class RateLimitValidationQueryProvider : IRateLimitValidationQueryProvider
{
    public IQueryable<UserActivity> ApplyFilter(IRateLimit rateLimit, IQueryable<UserActivity> queryable)
    {
        var utcNow = DateTime.UtcNow;
        switch (rateLimit)
        {
            case TimeFromLastCallRateLimit timeFromLastCallRateLimit:
                return queryable
                    .OrderByDescending(i => i.Timestamp)
                    .Take(1)
                    .Where(i => i.Timestamp + timeFromLastCallRateLimit.TimeFromLastCall >= utcNow);

            case RequestsPerLastTimeRateLimit requestsPerLastTimeRateLimit:
                return queryable
                    .Where(i => i.Timestamp > utcNow - requestsPerLastTimeRateLimit.TimeSpan)
                    .GroupBy(i => i.ApiKey)
                    .Where(i => i.Count() > requestsPerLastTimeRateLimit.MaxCallCount)
                    .SelectMany(i => i);

            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}