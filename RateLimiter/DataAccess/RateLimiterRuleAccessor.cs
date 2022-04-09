using RateLimiter.DataStorageSimulator;
using RateLimiter.Interfaces;
using RateLimiter.Models;
using System.Collections.Generic;

namespace RateLimiter.DataAccess
{
    /// <summary>
    /// Class that provides an access to the Rate Limiter Data Storage
    /// </summary>
    public class RateLimiterRuleAccessor : IRateLimiterRuleAccessor
    {      
        /// <summary>
        /// get rate limiter value per client
        /// </summary>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public List<RateLimiterTypeInfo> GetRateLimiterValuePerClient(Client clientId) 
        {
            return StorageSimulator.ClientRateLimitValuePerRateType(clientId, GetRateLimiterTypePerClient(clientId));
        }
        /// <summary>
        /// Returns rate limit per client
        /// </summary>
        /// <param name="clientId"></param>
        /// <returns></returns>
        private List<RateLimiterType> GetRateLimiterTypePerClient(Client clientId)
        {
            return StorageSimulator.RateLimitTypeToClient(clientId);
        }
    }
}
