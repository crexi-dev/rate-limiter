using System;
using System.Collections.Generic;
using System.Linq;

public class FixedWindowRateLimitRule(int maxRequests, TimeSpan timespan) : IRateLimitRule
{
    private readonly int _maxRequests = maxRequests;
    private readonly TimeSpan _timespan = timespan;

    public bool IsRequestAllowed(string client,
        string resource,
        DateTime currentTime,
        Dictionary<(string client, string resource), List<DateTime>> clientRequests)
    {
        var key = (client, resource);
        if (!clientRequests.ContainsKey(key))
        {
            return true;
        }

        var windowStartTime = currentTime - _timespan;
        var requestsInWindow = clientRequests[key].Where(req => req > windowStartTime).ToList();
        return requestsInWindow.Count < _maxRequests;
    }
}
