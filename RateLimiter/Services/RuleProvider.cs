using System;
using System.Collections.Generic;
using RateLimiter.Interfaces;

namespace RateLimiter.Services
{
    /// <summary>
    /// Provides functionality for managing and retrieving rate limiting rules for different resources and regions.
    /// </summary>
    public class RuleProvider : IRuleProvider
    {
        private readonly Dictionary<Tuple<string, string>, List<IRule>> _rules = new();
        public readonly IDateTimeWrapper _dateTimeWrapper;

        /// <inheritdoc />
        public RuleProvider(IDateTimeWrapper dateTimeWrapper)
        {
            _dateTimeWrapper = dateTimeWrapper ?? throw new ArgumentNullException(nameof(dateTimeWrapper));
        }

        /// <inheritdoc />
        public IRuleProvider AddRule(string resource, string region, IRule rule)
        {
            if (string.IsNullOrWhiteSpace(resource))
                throw new ArgumentException("Resource cannot be null or empty.", nameof(resource));
            if (string.IsNullOrWhiteSpace(region))
                throw new ArgumentException("Region cannot be null or empty.", nameof(region));
            if (rule is null)
                throw new ArgumentNullException(nameof(rule));

            var key = Tuple.Create(resource, region);
            if (!_rules.ContainsKey(key))
            {
                _rules[key] = new List<IRule>();
            }
            _rules[key].Add(rule);
            return this;
        }

        /// <inheritdoc />
        public List<IRule> GetRulesForResource(string resource, string region)
        {
            if (string.IsNullOrWhiteSpace(resource))
                throw new ArgumentException("Resource cannot be null or empty.", nameof(resource));
            if (string.IsNullOrWhiteSpace(region))
                throw new ArgumentException("Region cannot be null or empty.", nameof(region));

            var key = Tuple.Create(resource, region);
            return _rules.ContainsKey(key) ? _rules[key] : new List<IRule>();
        }
    }
}
