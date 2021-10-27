using System;

namespace RateLimiter.Domain.ApiLimiter
{
    public interface IRule : ICloneable
    {
        bool NewVisitAndRuleCheck();
    }
}