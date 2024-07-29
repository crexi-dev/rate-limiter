using System;
using System.Linq;
using RateLimiter.Models;

namespace RateLimiter.Rules;

public class TimePassed(long milliseconds) : IRule
{
    public bool HasAccess(string token, Guid resource, AccessStatistics statistics)
    {
        var last = statistics.AccessList.LastOrDefault();
        if (last == null)
        {
            return true;
        }
        return DateTimeOffset.Now - last.AccessTime > TimeSpan.FromMilliseconds(milliseconds);
    }
}