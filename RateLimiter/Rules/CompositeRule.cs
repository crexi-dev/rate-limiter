using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RateLimiter.Abstractions;
using RateLimiter.Models;

namespace RateLimiter.Rules
{
    public class CompositeRule : IRateLimitRule
    {
        private readonly List<IRateLimitRule> _rules = new List<IRateLimitRule>();
        
        public CompositeRule()
        {
        }
        
        public CompositeRule(IEnumerable<IRateLimitRule> rules)
        {
            if (rules == null)
            {
                throw new ArgumentNullException(nameof(rules));
            }
            
            _rules.AddRange(rules);
        }
        
        public void AddRule(IRateLimitRule rule)
        {
            if (rule == null)
            {
                throw new ArgumentNullException(nameof(rule));
            }
            
            _rules.Add(rule);
        }
        
        public async Task<bool> ValidateAsync(RequestContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }
            
            if (_rules.Count == 0)
            {
                return true;
            }
            
            foreach (var rule in _rules)
            {
                if (!await rule.ValidateAsync(context))
                {
                    return false;
                }
            }
            
            return true;
        }
    }
}