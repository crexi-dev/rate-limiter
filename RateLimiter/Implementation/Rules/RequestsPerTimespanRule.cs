using System;

namespace RateLimiter.Implementation.Rules;

public class RequestsPerTimespanRule : BaseRule 
{
    private readonly int _maxRequests;
    private readonly TimeSpan _timespan;

    public RequestsPerTimespanRule(int maxRequests, TimeSpan timespan)
    {
        _maxRequests = maxRequests;
        _timespan = timespan;
    }

    public override bool IsRequestAllowed(string clientId, string resource)
    {
        var now = DateTime.UtcNow;
        var requests = GetRequestCount(clientId, resource, now - _timespan, now);
        if (requests < _maxRequests)
        {
            SaveRequest(clientId, resource, now);
            return true;
        }
        return false;
    }
}