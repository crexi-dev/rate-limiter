using System;
using System.Collections.Generic;
using System.Linq;

namespace RateLimiter;

// Example rule where there are default values, 
// then region specific values,
// then paid service tiers values.

// The intent of the rule is to require a minimum amount of time to pass between allowed requests.
public class CooldownPeriod : IRequestValidatorRule
{
    private Dictionary<string, DateTime> _lastRequestTime { get; set; } = new();
    public bool ValidateRequest(UserSettings user, RuleSettings settings, ValidationRequest request)
    {

        // If we have no history, can't fail cooldown
        if (_lastRequestTime.ContainsKey(user.Id) == false)
        {
            _lastRequestTime[user.Id] = request.RequestTime;
            return true;
        }

        // Begin with default values
        var cooldown = Helpers.GetDefault(settings.Options)?.CooldownMsec;

        // Override with region values
        var regionSettings = Helpers.GetRegionOption(settings.Options, user);
        if (regionSettings != null)
        {
            cooldown = regionSettings.CooldownMsec;
        }

        // Override with tier values, if shorter (priority order is business decision)
        var tierSettings = Helpers.GetTierOption(settings.Options, user);
        if (tierSettings != null)
        {
            if (tierSettings.CooldownMsec < cooldown)
            {
                cooldown = tierSettings.CooldownMsec;
            }
        }

        // Compare request timestamp to last allowed.
        // Update log if allowed.
        if ((request.RequestTime - _lastRequestTime[user.Id]).TotalMilliseconds < cooldown)
        {
            return false;
        }
        else
        {
            _lastRequestTime[user.Id] = request.RequestTime;
            return true;
        }

    }
}