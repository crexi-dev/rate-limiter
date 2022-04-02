using RateLimiter.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RateLimiter
{
    public class RateLimitingValidation
    {
        private readonly RateLimitingConfigurations _rateLimitConfigurations;
        public RateLimitingValidation(RateLimitingConfigurations rateLimitConfigurations)
        {
            _rateLimitConfigurations = rateLimitConfigurations;
        }

        public bool ValidateRequest(RequestHistory requestHistory)
        {
            if (requestHistory == null)
            {
                throw new NullReferenceException("requestHistory");
            }
            if (requestHistory.History == null || requestHistory.History.Count == 0)
            {
                return true;
            }
            if (_rateLimitConfigurations.Combinations == null || _rateLimitConfigurations.Combinations.Count == 0)
            {
                foreach (var history in requestHistory.History)
                {
                    switch (history.Type)
                    {
                        case RateLimitingType.PerTime:
                            if (!ValidatePerTime(history, _rateLimitConfigurations.Configurations))
                            {
                                return false;
                            }
                            break;
                        case RateLimitingType.LastCall:
                            if (!ValidatePerTime(history, _rateLimitConfigurations.Configurations))
                            {
                                return false;
                            }
                            break;
                        default:
                            break;
                    }
                }
            }
            if (_rateLimitConfigurations.Combinations != null && _rateLimitConfigurations.Combinations.Count > 0)
            {
                foreach (var combination in _rateLimitConfigurations.Combinations)
                {
                    var configs = _rateLimitConfigurations.Configurations.Where(x => combination.Titles.Contains(x.Title));
                    if (configs.Any() && !ValidateCombined(configs, requestHistory.History, combination.CombinationType))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public bool ValidateCombined(IEnumerable<RateLimitConfiguration> configurations, IEnumerable<RateLimiting> limitHistory, RateLimitingCombinationType combinationType)
        {
            foreach (var history in limitHistory)
            {
                if (!(combinationType == RateLimitingCombinationType.And && ValidatePerTime(history, configurations) && ValidateLastCall(history, configurations)))
                {
                    return false;
                }
                if (combinationType == RateLimitingCombinationType.Or && !(ValidatePerTime(history, configurations) || ValidateLastCall(history, configurations)))
                {
                    return false;
                }
            }

            return true;
        }

        public bool ValidatePerTime(RateLimiting requestHistory, IEnumerable<RateLimitConfiguration> configurations)
        {
            return ValidatePerTime(new List<RateLimiting> { requestHistory }, configurations);
        }

        public bool ValidatePerTime(IEnumerable<RateLimiting> requestHistories, IEnumerable<RateLimitConfiguration> limitConfiguration)
        {
            var configurations = limitConfiguration.Select(x => x.Limits.Where(x => x.Type == RateLimitingType.LastCall));
            foreach (var limits in configurations)
            {
                foreach (var limit in limits)
                {
                    var histories = requestHistories.Where(x => x.Type == RateLimitingType.PerTime).Select(x => (long)x.Value).OrderByDescending(x => x);
                    if ((long)limit.Value < histories.First())
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public bool ValidateLastCall(RateLimiting requestHistory, IEnumerable<RateLimitConfiguration> limitConfiguration)
        {
            return ValidateLastCall(new List<RateLimiting> { requestHistory }, limitConfiguration);
        }

        public bool ValidateLastCall(IEnumerable<RateLimiting> requestHistories, IEnumerable<RateLimitConfiguration> limitConfiguration)
        {
            var configurations = limitConfiguration.Select(x => x.Limits.Where(x => x.Type == RateLimitingType.LastCall));
            foreach (var limits in configurations)
            {
                foreach (var limit in limits)
                {
                    var histories = requestHistories.Where(x => x.Type == RateLimitingType.PerTime).Select(x => (long)x.Value).OrderByDescending(x => x);
                    if ((long)limit.Value < histories.First())
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }
}
