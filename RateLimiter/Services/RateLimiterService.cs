using RateLimiter.DataAccess;
using RateLimiter.DataStorageSimulator;
using RateLimiter.Interfaces;
using RateLimiter.Models;
using RateLimiter.Validators;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace RateLimiter.Services
{
    /// <summary>
    /// Class to determine if request should be allowed to go through
    /// or should be declined following pre-defined Rate Limit rules
    /// </summary>
    public class RateLimiterService : IRateLimiterService
    {        
        private IRateLimiterRuleAccessor _rateLimiterRuleAccessor;
        private IClientManagerService _clientManagerService;        

        private static ConcurrentDictionary<Client, ConcurrentDictionary<AvailableResource, List<DateTime>>> _rateLimiterRequestCache = new ConcurrentDictionary<Client, ConcurrentDictionary<AvailableResource, List<DateTime>>>();
        private static ConcurrentDictionary<Client, List<RateLimiterTypeInfo>> _clientRateLimiterTypeInfoList = new ConcurrentDictionary<Client, List<RateLimiterTypeInfo>>();
        /// <summary>
        /// default constructor
        /// </summary>
        public RateLimiterService() : this(new ClientManagerService(), new RateLimiterRuleAccessor(), new RateLimiterValidatorFactory()) 
        {
            
        }
        /// <summary>
        /// DI components
        /// </summary>
        /// <param name="clientManagerService"></param>
        /// <param name="rateLimiterRuleAccessor"></param>
        public RateLimiterService(IClientManagerService clientManagerService, IRateLimiterRuleAccessor rateLimiterRuleAccessor, IRateLimiterValidatorFactory RateLimiterValidatorFactory) 
        {
            _rateLimiterRuleAccessor = rateLimiterRuleAccessor;
            _clientManagerService = clientManagerService;

            //initializing Client's Rate Limit Rules
            foreach (string clientName in Enum.GetNames(typeof(Client))) 
            {
                var clientId = (Client)Enum.Parse(typeof(Client), clientName);
                if (clientId == Client.UnRated) 
                {
                    continue;
                }
                var RateLimiterTypeInfoList = _rateLimiterRuleAccessor.GetRateLimiterValuePerClient(clientId);
                foreach (var rateLimiterType in RateLimiterTypeInfoList)
                {
                    rateLimiterType.RateLimiterValidator = RateLimiterValidatorFactory.CreateRateLimiterValidator(rateLimiterType.Type);
                }
                _clientRateLimiterTypeInfoList.TryAdd(clientId, RateLimiterTypeInfoList);
            }
        }
        /// <summary>
        /// Returns True if request is allowed per rate limit rules per client and resource
        /// Returns False if request is declined based on the rate limit rules per client and resource
        /// </summary>
        /// <param name="token"></param>
        public bool IsRequestAllowed(Token token, AvailableResource resource) 
        {
            var clientId = _clientManagerService.GetRatedClientIdByToken(token);
            if (clientId == Client.UnRated || _clientRateLimiterTypeInfoList[clientId].Where(x => x.ResourceName == resource).Count() == 0) 
            {
                return true;
            }

            var requestTime = DateTime.UtcNow;
            if (!_rateLimiterRequestCache.ContainsKey(clientId))
            {
                var resourceRequestList = new ConcurrentDictionary<AvailableResource, List<DateTime>>();
                resourceRequestList.TryAdd(resource, new List<DateTime> { requestTime });                
                _rateLimiterRequestCache.TryAdd(clientId, resourceRequestList);
                return true;
            }
            else
            {
                if (!_rateLimiterRequestCache[clientId].ContainsKey(resource))
                {
                    _rateLimiterRequestCache[clientId].TryAdd(resource, new List<DateTime> { requestTime }); 
                    return true;
                }

                var clientRateLimiterRuleList = _clientRateLimiterTypeInfoList[clientId];
                try 
                {
                    foreach (var clientRateLimiterInfo in clientRateLimiterRuleList)
                    {
                        if (clientRateLimiterInfo.RateLimiterValidator != null)
                        {
                            if (clientRateLimiterInfo.ResourceName == resource)
                            {
                                if (!clientRateLimiterInfo.RateLimiterValidator.ValidateRateLimitRule(clientId, requestTime, clientRateLimiterInfo,
                                    _rateLimiterRequestCache[clientId][resource]))
                                {
                                    return false;
                                }
                            }
                        }
                    }
                    return true;
                }
                finally 
                {
                    var maxTimeIntervalToKeep = clientRateLimiterRuleList.Max(x => x.TimeInterval);
                    _rateLimiterRequestCache[clientId][resource].RemoveAll(x => DateTime.Compare(x, requestTime.Subtract(2 * maxTimeIntervalToKeep)) < 0);
                    _rateLimiterRequestCache[clientId][resource].Add(requestTime);
                }
            }
        }
    }
}
