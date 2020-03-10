using System;
using System.Collections.Generic;

namespace RateLimiter.Rules
{
    public class RateLimitRuleFactory : IRateLimitRuleFactory
    {
        public List<IRateLimitRule> GetRateLimitRulesByResource(string resourceName)
        {
            var rulesToExecute = new List<IRateLimitRule>();

            switch (resourceName.ToLower())
            {
                case "resourcea":
                    rulesToExecute.Add(new MaxRequestsPerTimespanRateLimitRule());
                    break;
                case "resourceb":
                    rulesToExecute.Add(new TimespanPassedRateLimitRule());
                    break;
                case "resourcec":
                    rulesToExecute.Add(new MaxRequestsPerTimespanRateLimitRule());
                    rulesToExecute.Add(new TimespanPassedRateLimitRule());
                    break;
                default:
                    throw new Exception($"Resource {resourceName} not found");
            }

            return rulesToExecute;
        }
    }
}
