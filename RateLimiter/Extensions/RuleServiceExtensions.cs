using System;
using RateLimiter.Rules;
using RateLimiter.Services;

namespace RateLimiter.Extensions
{
    public static class RuleServiceExtensions
    {
        public static RuleService ForResource(this RuleService ruleService, string resource)
        {
            return ruleService.ConfigureResource(resource);
        }

        public static RuleService ForRegion(this RuleService ruleService, string region)
        {
            return ruleService.ForRegion(region);
        }

        public static RuleService XRequestsPerTimespan(this RuleService ruleService, int requestLimit, TimeSpan timeSpan)
        {
            return ruleService.AddRule(new XRequestsPerTimespanRule(requestLimit, timeSpan));
        }

        public static RuleService TimeSinceLastCall(this RuleService ruleService, TimeSpan timeSpan)
        {
            return ruleService.AddRule(new TimeSinceLastCallRule(timeSpan));
        }
    }

}
