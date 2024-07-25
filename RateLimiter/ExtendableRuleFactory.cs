using RateLimiter.Enums;
using RateLimiter.Interfaces;
using RateLimiter.Rules;
using System;
using System.Collections.Generic;

namespace RateLimiter
{
    public class ExtendableRuleFactory : IExtendableRuleFactory
    {
        private List<IRateLimiter> rulesList = new();

        public List<IRateLimiter> GetRulesByServiceType(ServiceType serviceType)
        {
            switch (serviceType)
            {
                case ServiceType.GenerateLabelService:
                    rulesList.Add(new MaxRequestsPerTimespanRateLimiter());
                    break;
                case ServiceType.LabelAdjustmentService:
                    rulesList.Add(new TimespanPassedRateLimiter());
                    break;
                case ServiceType.RefundLabelService:
                    rulesList.Add(new TokenBucketRateLimiter());
                    break;
                case ServiceType.EmailService:
                    rulesList.Add(new MaxRequestsPerTimespanRateLimiter());
                    rulesList.Add(new TimespanPassedRateLimiter());
                    break;

                default:
                    throw new Exception("Unknown service type");
            }

            return rulesList;
        }
    }
}
