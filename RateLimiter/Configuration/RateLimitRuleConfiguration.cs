using RateLimiter.Interfaces.Configuration;
using System.Collections.Generic;

namespace RateLimiter.Configuration
{
    public class RateLimitRuleConfiguration : IRateLimitRuleConfiguration
    {
        public string Type { get; init; }

        public IDictionary<string, object> Parameters { get; init; }
    }
}
