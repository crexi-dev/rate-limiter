using RateLimiter.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RateLimiter
{
    public class Resource : IResouce
    {
        private readonly Stack<DateTimeOffset> _requestTimes;
        private readonly HashSet<IRule> _rules;

        private Resource(Stack<DateTimeOffset> requestTimes, HashSet<IRule> rules)
        {
            _requestTimes = requestTimes;
            _rules = rules;
        }

        public IResouce AddRule(IRule rule)
        {
            _rules.Add(rule);
            return this;
        }

        public static IResouce Create(Stack<DateTimeOffset> requestTimes)
        {
            return new Resource(requestTimes, new HashSet<IRule>());
        }

        public bool CanGoThrough(DateTimeOffset requestDateTimeOffset)
        {
            var previous = _requestTimes.ToArray();
            _requestTimes.Push(requestDateTimeOffset);

            foreach (var rule in _rules)
            {
                if (!rule.Verify(new Stack<DateTimeOffset>(previous.Reverse()), requestDateTimeOffset))
                {
                    _requestTimes.Pop();
                    return false;
                }
            }

            return true;
        }
    }
}
