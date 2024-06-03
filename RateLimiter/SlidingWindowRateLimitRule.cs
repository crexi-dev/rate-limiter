using System;
using System.Collections.Generic;
using System.Linq;

public class SlidingWindowRateLimitRule(TimeSpan minInterval) : IRateLimitRule
{
    private readonly TimeSpan _minInterval = minInterval;

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

        var lastRequestTime = clientRequests[key].LastOrDefault();
        return lastRequestTime == default || currentTime - lastRequestTime >= _minInterval;
    }
}
