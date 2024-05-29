using System;
using RateLimiter.Interfaces;
using RateLimiter.Rules;
using RateLimiter.Services;

namespace RateLimiter.Extensions
{
    public static class RuleServiceExtensions
    {
        public static RuleProvider AddRuleForResource(this RuleProvider ruleService, string resource, string region, IRule rule)
        {
            return ruleService.AddRule(resource, region, rule);
        }

        public static RuleProvider ApplyXRequestsPerTimespanRule(this RuleProvider ruleService, string resource, string region, int requestLimit, TimeSpan timeSpan)
        {
            return ruleService.AddRule(resource, region, new XRequestsPerTimespanRule(requestLimit, timeSpan, ruleService._dateTimeWrapper));
        }

        public static RuleProvider ApplyTimeSinceLastCallRule(this RuleProvider ruleService, string resource, string region, TimeSpan timeSpan)
        {
            return ruleService.AddRule(resource, region, new TimeSinceLastCallRule(timeSpan, ruleService._dateTimeWrapper));
        }
    }
}
