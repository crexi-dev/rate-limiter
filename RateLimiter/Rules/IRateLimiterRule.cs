using RateLimiter.Models;
using System.Collections.Generic;

namespace RateLimiter.Rules
{
    public interface IRateLimiterRule
    {
        bool IsRequestAllowed(RequestModel currentRequest, List<RequestModel> requests, RuleModel rule);
    }
}