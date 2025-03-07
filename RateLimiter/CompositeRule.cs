/*
 * Author   Joel Hernandez James
 * Current Date  3/6/2025
 * Class    CompositeRule
 */

using System;
using System.Collections.Generic;
using System.Linq;

namespace RateLimiter
{
    /// <summary>
    /// A rule that bundles multiple rules together and applies them as a group.
    /// Think of it like a parent rule that manages a collection of child rules.
    /// </summary>
    public class CompositeRule : IRateLimitRule
    {
        /// <summary>
        /// Ways to combine multiple rules - like choosing between "all must pass" or "any can pass"
        /// </summary>
        public enum LogicalOperator
        {
            /// <summary>
            /// All rules must say "yes" for the request to be allowed (like a unanimous vote)
            /// </summary>
            And,
            
            /// <summary>
            /// At least one rule must say "yes" for the request to be allowed (like needing just one vote)
            /// </summary>
            Or
        }

        private readonly List<IRateLimitRule> _rules;
        private readonly LogicalOperator _operator;

        /// <summary>
        /// Creates a new rule group with the specified way of combining results
        /// </summary>
        /// <param name="operator">How to combine the results - "And" means all must pass, "Or" means any can pass</param>
        public CompositeRule(LogicalOperator @operator)
        {
            _rules = new List<IRateLimitRule>();
            _operator = @operator;
        }

        /// <summary>
        /// Adds a rule to our collection of rules
        /// </summary>
        /// <param name="rule">The rule to add to the group</param>
        public void AddRule(IRateLimitRule rule)
        {
            if (rule == null)
                throw new ArgumentNullException(nameof(rule));

            _rules.Add(rule);
        }

        /// <summary>
        /// Checks if a request is allowed by applying all our rules and combining the results
        /// </summary>
        /// <param name="token">Who's making the request (their ID)</param>
        /// <param name="resourceId">What they're trying to access</param>
        /// <returns>Yes (true) if allowed based on our combining method, No (false) if blocked</returns>
        public bool IsRequestAllowed(string token, string resourceId)
        {
            // No rules? No problem! Let them through
            if (_rules.Count == 0)
                return true;

            // Apply our rules based on how we want to combine them
            switch (_operator)
            {
                case LogicalOperator.And:
                    // Everyone must agree - if any rule says no, the answer is no
                    return _rules.All(rule => rule.IsRequestAllowed(token, resourceId));
                
                case LogicalOperator.Or:
                    // Just need one yes - if any rule says yes, the answer is yes
                    return _rules.Any(rule => rule.IsRequestAllowed(token, resourceId));
                
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
