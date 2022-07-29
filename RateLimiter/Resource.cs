using System.Collections.Generic;
using RateLimiter.Rules;

namespace RateLimiter
{
    public class Resource
    {
        private readonly List<IRule> _rules = new();

        public string Path { get; }

        public Resource(string path)
        {
            Path = path;
        }
        
        public void ApplyRule(IRule rule)
        {
            _rules.Add(rule);
        }

        public List<IRule> GetRules()
        {
            return _rules;
        }
    }
}
