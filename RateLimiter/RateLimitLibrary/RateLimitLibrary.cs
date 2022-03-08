using System;
using Microsoft.Extensions.Options;
using System.Collections.Generic;

namespace RateLimiter
{
    public class RateLimitLibrary : IRateLimitLibrary
    {
        private readonly Settings? _settings;
        public RateLimitLibrary(IOptions<Settings> settings)
        {
            _settings = settings?.Value ?? throw new ArgumentNullException(nameof(settings));
        }

        public bool IsRateLimitAccepted(string userToken, string apiName)
        {
            // 1. Find user log time history
            List<DateTime> userApiLogTimes = DataSimulator.GetUserApiLogTimes(userToken, apiName);

            // 2. Add current time to list and save
            userApiLogTimes.Add(DateTime.Now);
            DataSimulator.SaveOrUpdateApiLog(userToken, apiName, userApiLogTimes);

            // 3. Find out if we are under the rate limit for any rules defined for this api resource
            List<RuleDefinition>? rules = _settings?.GetRulesToApply(apiName);

            return isUnderRateLimit(rules ?? throw new ArgumentNullException(nameof(rules)), userApiLogTimes);
        }
        
        private bool isUnderRateLimit(List<RuleDefinition> rules, List<DateTime> userApiLogTimes)
        {
            foreach(RuleDefinition rule in rules)
            {
                int numberOfLogsInTimeSpan = userApiLogTimes.FindAll(logTime => logTime > DateTime.Now.AddSeconds(-rule.timeSpanSeconds)).Count;
                if (numberOfLogsInTimeSpan > rule.allowedRequests)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
