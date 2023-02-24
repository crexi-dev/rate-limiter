using System.Collections.Concurrent;
using System.Runtime.InteropServices.ComTypes;

namespace RateLimiter.Configuration
{
    public class RateLimiterOptions
    {
        private readonly ConcurrentDictionary<string, ResourceRuleSet> _rules = new ();

        public IResourceRuleSet For(string resourceName)
        {
            return _rules.GetOrAdd(resourceName, key => new ResourceRuleSet());
        }
    }
}