using RateLimiter.Model.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace RateLimiter.ApiRule.Factory
{

    public interface IRateLimitedFactory
    {
        List<IRuleValidation> GetRateLimitRulesByResource(ResourceEnum resourceName);
    }
}
