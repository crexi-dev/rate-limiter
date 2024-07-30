using System;
using System.Collections.Generic;
using System.Linq;


namespace RateLimiter;

// The goal of this rule is to limit requests within a certain time window,
// for example 100 requests in 500 milliseconds.
public class MaxRequestsRollingWindow : IRequestValidatorRule
{
    // All insertions and removals are at front or end of list
    private Dictionary<string, LinkedList<DateTime>> _timeStamps { get; set; } = new();

    public bool ValidateRequest(UserSettings user, RuleSettings settings, ValidationRequest request)
    {
        // Defaults are required
        var defaultSettings = Helpers.GetDefault(settings.Options);
        if (defaultSettings == null || defaultSettings.MaxRequests == null || defaultSettings.PeriodMsec == null)
        {
            return false;
        }

        // Set defaults
        int maxRequests = (int)defaultSettings.MaxRequests;
        int lookbackMsec = (int)defaultSettings.PeriodMsec;

        // Override with region settings, if found
        var regionSettings = Helpers.GetRegionOption(settings.Options, user);
        if (regionSettings != null)
        {
            if (regionSettings.MaxRequests != null)
            {
                maxRequests = (int)regionSettings.MaxRequests;
            }
            if (regionSettings.PeriodMsec != null)
            {
                lookbackMsec = (int)regionSettings.PeriodMsec;
            }
        }

        // Remove timestamps older than lookback window
        if (_timeStamps.ContainsKey(user.Id))
        {
            var current = _timeStamps[user.Id].First;
            while (current != null && current.Value < request.RequestTime.AddMilliseconds(-lookbackMsec))
            {
                var next = current.Next;
                _timeStamps[user.Id].Remove(current);
                current = next;
            }
        }
        else
        {
            _timeStamps[user.Id] = new LinkedList<DateTime>();
        }

        // Count remaining timestamps. Are we under threshold?
        if (_timeStamps[user.Id].Count < maxRequests)
        {
            _timeStamps[user.Id].AddLast(request.RequestTime); 
            return true;
        }
        else
        {
            return false;
            // We don't add the timestamp because we're only counting allowed requests against the quota.
            // It's arguable business logic, but idea is something like:
            // If I'm allowed 10 requests per 10 minutes, but make 10 requests every minute,
            // I should succeed 60 times an hour out of 600 requests -- not succeed my very first 10 times and then never again.
        }
    }
}