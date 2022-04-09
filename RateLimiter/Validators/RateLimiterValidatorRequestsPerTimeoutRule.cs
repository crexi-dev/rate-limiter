using RateLimiter.DataStorageSimulator;
using RateLimiter.Interfaces;
using RateLimiter.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RateLimiter.Validators
{
    class RateLimiterValidatorRequestsPerTimeoutRule : IRateLimiterValidator
    {
        bool IRateLimiterValidator.ValidateRateLimitRule(Client clientId, DateTime requestTime, RateLimiterTypeInfo clientRateLimiterInfo, List<DateTime> clientHistoryRequestTime)
        {
            return requestTime.Subtract(clientHistoryRequestTime.Max()) > clientRateLimiterInfo.TimeInterval;
        }
    }
}
