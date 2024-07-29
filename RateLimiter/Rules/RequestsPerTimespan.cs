using System;
using System.Linq;
using RateLimiter.Models;

namespace RateLimiter.Rules;

public class RequestsPerTimespan(long requestsCount, long milliseconds) : IRule
{
    public bool HasAccess(string token, Guid resource, AccessStatistics statistics)
    {
        var requestsPerTimespan =
            statistics.AccessList.Count(q => q.AccessTime >= DateTimeOffset.Now.AddMilliseconds(-milliseconds));
        return requestsPerTimespan < requestsCount;
    }
}