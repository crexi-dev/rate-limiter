using System;
using System.Collections.Concurrent;
using Microsoft.AspNetCore.Routing;
using System.Collections.Generic;
using System.Linq;

namespace RateLimiter.Core.Rules
{
    public class CompositeAndRule : IRateLimitRule
    {
        private readonly List<IRateLimitRule> _rules;

        public CompositeAndRule(params IRateLimitRule[] rules)
        {
            _rules = rules.ToList();
        }

        public bool IsAllowed(string clientToken, string resourceId)
        {
            // all rules must allow the request
            return _rules.All(rule => rule.IsAllowed(clientToken, resourceId));
        }
    }
}