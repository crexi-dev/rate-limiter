using System.Collections.Generic;

namespace RateLimiter.Configuration
{
    internal class ResourceRuleSet : IResourceRuleSet
    {
        private readonly List<IRule> _rules = new();
        public IResourceRuleSet AddRule(IRule rule)
        {
            _rules.Add(rule);

            return this;
        }
    }
}