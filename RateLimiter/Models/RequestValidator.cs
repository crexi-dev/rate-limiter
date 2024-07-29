using System;
using System.Collections.Generic;
using System.Linq;

namespace RateLimiter;

// API USER CONFIGURATION
// We are going to assume that API user settings are stored per key
// and passed into the Validator method per request.
// For example, could be added to the http request by the API gateway at time of key validation


// RATE LIMITER RULE CONFIGURATION
// Configuration would be set up by the host, 
// for example using dependency injection with IConfiguration and appsettings.json,
// or cloud platform env configuration manager.


// LIFECYCLE AND CONCURRENCY
// Because memory is in rule instance, for history to work you'd need to pass in the same rule instance.

public class RequestValidator
{
    public bool CheckRules(UserSettings user, RuleSettingsList settingsList, List<IRequestValidatorRule> rules, ValidationRequest request)
    {
        foreach (var rule in rules)
        {
            // Class and configuration rule names must match.
            var settings = settingsList.RateLimiterRules?.Where(r => r.Name == rule.GetType().Name).FirstOrDefault();
            // Configuration is required.
            if (settings == null || settings.Options == null)
            {
                return false;
            }
            // If rule is not enabled, skip it
            if (settings.Enabled == false)
            {
                continue;
            }
            // Now run each rule
            var isValid = rule.ValidateRequest(user, settings, request);
            if (isValid == false)
            {
                return false;
            }
        }

        return true;
    }

}

