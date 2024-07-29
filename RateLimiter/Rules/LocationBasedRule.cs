using System;
using RateLimiter.Models;

namespace RateLimiter.Rules;

public class LocationBasedRule(long requestsPerTimeSpan, long timeSpan, long timePassed) : IRule
{
    private readonly RequestsPerTimespan _requestsPerTimespan = new (requestsPerTimeSpan, timeSpan);
    private readonly TimePassed _timePassed = new (timePassed);
    
    public bool HasAccess(string token, Guid resource, AccessStatistics statistics)
    {
        return token.IsUsBased()
            ? _requestsPerTimespan.HasAccess(token, resource, statistics)
            : _timePassed.HasAccess(token, resource, statistics);
    }
}