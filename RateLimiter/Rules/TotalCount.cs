using System;
using RateLimiter.Models;

namespace RateLimiter.Rules;

public class TotalCount(long count) : IRule
{
    public bool HasAccess(string token, Guid resource, AccessStatistics statistics)
    {
        return statistics.AccessList.Count < count;
    }
}