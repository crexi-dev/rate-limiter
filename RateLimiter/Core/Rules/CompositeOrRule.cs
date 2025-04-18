using System;
using System.Collections.Concurrent;
using Microsoft.AspNetCore.Routing;
using System.Collections.Generic;
using System.Linq;

namespace RateLimiter.Core.Rules
{
    public class CompositeOrRule : IRateLimitRule
    {
        private readonly List<IRateLimitRule> _rules;

        public CompositeOrRule(params IRateLimitRule[] rules)
        {
            _rules = rules.ToList();
        }

        public bool IsAllowed(string clientToken, string resourceId)
        {
            // at least one rule must allow the request
            return _rules.Any(rule => rule.IsAllowed(clientToken, resourceId));
        }
    }
}