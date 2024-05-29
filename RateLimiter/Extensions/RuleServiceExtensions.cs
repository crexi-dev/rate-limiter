using System;
using RateLimiter.Rules;
using RateLimiter.Services;

namespace RateLimiter.Extensions
{
    public static class RuleServiceExtensions
    {
        public static RuleProvider ForResource(this RuleProvider ruleService, string resource)
        {
            return ruleService.ConfigureResource(resource);
        }

        public static RuleProvider ApplyXRequestsPerTimespanRule(this RuleProvider ruleService, int requestLimit, TimeSpan timeSpan)
        {
            return ruleService.AddRule(new XRequestsPerTimespanRule(requestLimit, timeSpan, ruleService._dateTime));
        }

        public static RuleProvider ApplyTimeSinceLastCallRule(this RuleProvider ruleService, TimeSpan timeSpan)
        {
            return ruleService.AddRule(new TimeSinceLastCallRule(timeSpan, ruleService._dateTime));
        }
    }

}
