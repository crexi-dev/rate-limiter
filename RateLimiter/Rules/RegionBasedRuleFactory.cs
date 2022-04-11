using System;
using System.Collections.Generic;
using System.Text;

namespace RateLimiter.Rules
{
    public interface IRegionBasedRuleFactory
    {
        IRateLimiterRule GetRateLimiterRule(string region);
    }

    public class RegionBasedRuleFactory : IRegionBasedRuleFactory
    {
        private readonly IServiceProvider serviceProvider;

        public RegionBasedRuleFactory(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public IRateLimiterRule GetRateLimiterRule(string region)
        {
            return (region.ToLower()) switch
            {
                "us" => (IRateLimiterRule)serviceProvider.GetService(typeof(RequestsPerTimespanRule)),
                "eu" => (IRateLimiterRule)serviceProvider.GetService(typeof(TimePassedRule)),
                _ => throw new Exception($"Unknown region entered. Region: {region}"),
            };
        }
    }
}
