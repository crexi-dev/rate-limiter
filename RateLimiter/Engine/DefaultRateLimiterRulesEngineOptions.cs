using RateLimiter.Interfaces;
using RateLimiter.Rules;

namespace RateLimiter.Engine
{
    /// <summary>
    /// A service to provide the default list of rules for the Rules Engine
    /// </summary>
    internal class DefaultRateLimiterRulesEngineOptions : IRateLimiterRulesEngineOptions
    {
        public DefaultRateLimiterRulesEngineOptions(RequestsPerTimespanRule requestsPerTimespanRule, TimeSinceLastRequestRule timeSinceLastRequestRule, 
            FallbackRule fallbackRule)
        {
            this.Rules = new RateLimiterBaseRule[]
            {
                requestsPerTimespanRule,
                timeSinceLastRequestRule,
                fallbackRule,
            };
        }

        public RateLimiterBaseRule[] Rules { get; }
    }
}
