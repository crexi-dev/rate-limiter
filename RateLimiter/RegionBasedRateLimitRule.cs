using System;
using System.Collections.Generic;

public class RegionBasedRateLimitRule(Dictionary<string, IRateLimitRule> regionRules) : IRateLimitRule
{
    private readonly Dictionary<string, IRateLimitRule> _regionRules = regionRules;

    public bool IsRequestAllowed(string client, 
        string resource, 
        DateTime currentTime, 
        Dictionary<(string client, string resource), List<DateTime>> clientRequests)
    {
        var region = GetRegionFromClient(client);
        if (_regionRules.ContainsKey(region))
        {
            return _regionRules[region]
                .IsRequestAllowed(client, resource, currentTime, clientRequests);
        }

        // Default to allowing the request if no specific rule for the region
        return true;
    }

    private string GetRegionFromClient(string client)
    {
        // This is just a placeholder implementation.
        return client.StartsWith("US") ? "US" : "EU";
    }
}
