using RateLimiter.DataStorageSimulator;
using System.Collections.Generic;
using RateLimiter.Models;

namespace RateLimiter.Interfaces
{
    /// <summary>
    /// Accessor to abstract data storage from service layer
    /// </summary>
    public interface IRateLimiterRuleAccessor
    {
        List<RateLimiterTypeInfo> GetRateLimiterValuePerClient(Client clientId);
    }
}
