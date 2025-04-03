using System;
using System.Collections.Generic;
using RateLimiter.Abstractions;

namespace RateLimiter.Configuration
{
    public class RateLimitOptions
    {
        public List<IRateLimitRule> DefaultRules { get; set; } = new List<IRateLimitRule>();
        
        public Dictionary<string, List<IRateLimitRule>> EndpointRules { get; set; } =
            new Dictionary<string, List<IRateLimitRule>>(StringComparer.OrdinalIgnoreCase);
        
        public IEnumerable<IRateLimitRule> GetRulesForEndpoint(string path)
        {
            foreach (var rule in DefaultRules)
            {
                yield return rule;
            }
            
            foreach (var kvp in EndpointRules)
            {
                if (path.StartsWith(kvp.Key, StringComparison.OrdinalIgnoreCase))
                {
                    foreach (var rule in kvp.Value)
                    {
                        yield return rule;
                    }
                }
            }
        }
    }
}