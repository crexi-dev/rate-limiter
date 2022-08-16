using RateLimiter.Models.Configuration.Rules;
using RateLimiter.Models.Rules;
using System;

namespace RateLimiter.Factories
{
    public sealed class RuleFactory
    {
        public IRule Create(RuleConfiguration ruleConfiguration)
        {
            return ruleConfiguration switch
            {
                RequestsPerTimeSpanRuleConfiguration requestsPerTimeSpanRuleConfiguration => 
                    new RequestsPerTimeSpanRule(requestsPerTimeSpanRuleConfiguration.MaxRequests, requestsPerTimeSpanRuleConfiguration.TimeSpanInSeconds),
                TimeSpanSinceLastRequestRuleConfiguration timeSpanSinceLastRequestRuleConfiguration => 
                    new TimeSpanSinceLastRequestRule(timeSpanSinceLastRequestRuleConfiguration.TimeSpanSinceLastRequestInSeconds),
                TimeSpanSinceLastSuccessfulRequestRuleConfiguration timeSpanSinceLastSuccessfulRequestRuleConfiguration => 
                    new TimeSpanSinceLastSuccessfulRequestRule(timeSpanSinceLastSuccessfulRequestRuleConfiguration.TimeSpanSinceLastSuccessfulRequestInSeconds),
                _ => throw new InvalidCastException()
            };
        }
    }
}
