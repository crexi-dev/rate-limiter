using System.Collections.Generic;
using System.Linq;

namespace RateLimiter
{
    /// <summary>
    /// This class manages the application of multiple rate limiting rules to different endpoints
    /// </summary>
    public class RateLimitRuleManager
    {
        private readonly Dictionary<string, List<IRateLimitRule>> _endpointRules;

        public RateLimitRuleManager()
        {
            _endpointRules = new Dictionary<string, List<IRateLimitRule>>();
        }

        /// <summary>
        /// Configure a specific endpoint to use a rate limiter rule.
        /// </summary> 
        public void AddRule(string endpoint, IRateLimitRule rule)
        {
            if (!_endpointRules.ContainsKey(endpoint))
            {
                _endpointRules[endpoint] = new List<IRateLimitRule>();
            }

            _endpointRules[endpoint].Add(rule);
        }

        /// <summary>
        /// Checks if rate is exceeded threshold of all rate limiter rules configured for that endpoint.
        /// </summary> 
        public bool IsLimitExceeded(string endpoint, string clientIdentifier)
        {
            if (!_endpointRules.ContainsKey(endpoint))
            {
                return false;
            }

            return _endpointRules[endpoint].Any(rule => rule.IsLimitExceeded(endpoint, clientIdentifier));
        }

        /// <summary>
        /// Records requests to be used in rate limiting decision
        /// </summary> 
        public void RecordRequest(string endpoint, string clientIdentifier)
        {
            if (!_endpointRules.ContainsKey(endpoint))
            {
                return;
            }

            foreach (var rule in _endpointRules[endpoint])
            {
                rule.RecordRequest(endpoint, clientIdentifier);
            }
        }
    }

}
