using System;
using System.Collections.Concurrent;

namespace RateLimiter.Configuration
{
    public class RateLimiterOptions
    {
        private readonly ConcurrentDictionary<string, ResourceRuleSet> _rules = new ();

        public IResourceRuleSet For(string resourceName)
        {
            if (string.IsNullOrWhiteSpace(resourceName))
            {
                throw new ArgumentNullException(nameof(resourceName));
            }

            return _rules.GetOrAdd(resourceName, key => new ResourceRuleSet());
        }

        internal ResourceRuleSet? GetRuleSet(string resource)
        {
            return _rules.TryGetValue(resource, out var set) ? set : null;
        }
    }
}