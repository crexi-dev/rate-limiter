using System;

namespace RateLimiter.Rules
{
    public interface IRulesEvaluator
    {
        bool CanAccess(DateTime start, DateTime last, int totalCalls);
    }
}