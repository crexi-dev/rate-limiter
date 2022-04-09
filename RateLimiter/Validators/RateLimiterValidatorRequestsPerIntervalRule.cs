using RateLimiter.DataStorageSimulator;
using RateLimiter.Interfaces;
using RateLimiter.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RateLimiter.Validators
{
    class RateLimiterValidatorRequestsPerIntervalRule : IRateLimiterValidator
    {
        bool IRateLimiterValidator.ValidateRateLimitRule(Client clientId, DateTime requestTime, RateLimiterTypeInfo clientRateLimiterInfo,
            List<DateTime> clientHistoryRequestTime)
        {
            var count = clientHistoryRequestTime.Where(x => DateTime.Compare(x, requestTime.Subtract(clientRateLimiterInfo.TimeInterval)) > 0 &&
                        DateTime.Compare(x, requestTime) < 0).Count();

            return count < clientRateLimiterInfo.RequestLimit;
        }
    }
}
