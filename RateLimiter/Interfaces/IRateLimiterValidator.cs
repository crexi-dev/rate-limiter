using RateLimiter.DataStorageSimulator;
using RateLimiter.Models;
using System;
using System.Collections.Generic;

namespace RateLimiter.Interfaces
{
    /// <summary>
    /// Rate Limiter Validator interface
    /// </summary>
    public interface IRateLimiterValidator
    {
        bool ValidateRateLimitRule(Client clientId, DateTime requestTime, RateLimiterTypeInfo clientRateLimiterInfo, List<DateTime> clientHistoryRequestTime);
    }


}
