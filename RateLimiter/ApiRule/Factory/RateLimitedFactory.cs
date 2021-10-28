using RateLimiter.ApiRule.Factory;
using RateLimiter.ApiRule.RuleImplementations;
using RateLimiter.Model.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace RateLimiter.ApiRule.Factory
{
    public class RateLimitedFactory : IRateLimitedFactory
    {
        public List<IRuleValidation> GetRateLimitRulesByResource(ResourceEnum resourceName)
        {
            var rulesToExecute = new List<IRuleValidation>();

            switch (resourceName)
            {
                case ResourceEnum.resourcea:
                    rulesToExecute.Add(new MaxRequestsPerTimespanRateLimitRule());
                    break;
                case ResourceEnum.resourceb:
                    rulesToExecute.Add(new TimespanPassedRateLimitRule());
                    break;
                case ResourceEnum.resourcec:
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


