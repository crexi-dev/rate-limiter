using RateLimiter.Models.Configuration;
using RateLimiter.Models.Configuration.Conditions;
using RateLimiter.Models.Configuration.Rules;
using System.Collections.Generic;

namespace RateLimiter.Builders.Configuration
{
    public sealed class PolicyConfigurationBuilder
    {
        private RuleConfiguration rule;
        private List<ConditionConfiguration> conditions = new List<ConditionConfiguration>();

        public PolicyConfigurationBuilder WithRule(RuleConfiguration rule)
        {
            this.rule = rule;
            return this;
        }

        public PolicyConfigurationBuilder WithRule(ConditionConfiguration condition)
        {
            conditions.Add(condition);
            return this;
        }

        public PolicyConfiguration Build() => new PolicyConfiguration
        {
            Rule = rule,
            Conditions = conditions
        };
    }
}
