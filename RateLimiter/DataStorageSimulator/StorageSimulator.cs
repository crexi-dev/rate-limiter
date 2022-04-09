using RateLimiter.Models;
using RateLimiter.Services;
using System.Collections.Generic;

namespace RateLimiter.DataStorageSimulator
{
    /// <summary>
    /// Class that simulates Rate Limiter Rules Data Storage
    /// This can be flat table, database, configuration file etc. etc.
    /// </summary>
    public static class StorageSimulator
    {
        /// <summary>
        /// Returns Rate Lmiter rule per client Id
        /// </summary>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public static List<RateLimiterType> RateLimitTypeToClient(Client clientId)
        {
            var result = new List<RateLimiterType>();
            switch (clientId) 
            {
                case Client.AClient:
                    {
                        result.Add(RateLimiterType.rlRequestsPerTimeout);
                        return result; 
                    }
                case Client.BClient:
                    {
                        result.Add(RateLimiterType.rlRequestsPerInterval);
                        return result;
                    }
                case Client.CClient:
                    {
                        result.Add(RateLimiterType.rlRequestsPerTimeout);
                        result.Add(RateLimiterType.rlRequestsPerInterval);
                        return result;
                    }
                default:
                    {
                        result.Add(RateLimiterType.rlUnknown);
                        return result;
                    }
            }
        }
        /// <summary>
        /// Returns preconfigured actual values
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="rateLimiterType"></param>
        /// <returns></returns>
        public static List<RateLimiterTypeInfo> ClientRateLimitValuePerRateType(Client clientId, List<RateLimiterType> rateLimiterTypeList) 
        {
            var result = new List<RateLimiterTypeInfo>();
            switch (clientId) 
            {
                case Client.AClient:
                    {
                        foreach (var rateLimiterType in rateLimiterTypeList)
                        {
                            switch (rateLimiterType)
                            {
                                case RateLimiterType.rlRequestsPerInterval:
                                    {
                                        result.Add(new RateLimiterTypeInfo
                                        {                                            
                                            Type = RateLimiterType.rlRequestsPerInterval,
                                            TimeInterval = new System.TimeSpan(0, 0, 1),
                                            RequestLimit = 10
                                        });
                                        break;
                                    }
                                case RateLimiterType.rlRequestsPerTimeout:
                                    {
                                        result.Add( new RateLimiterTypeInfo
                                        {
                                            ResourceName = AvailableResource.ResourceOne,
                                            Type = RateLimiterType.rlRequestsPerTimeout,
                                            TimeInterval = new System.TimeSpan(0, 0, 1),
                                            RequestLimit = 10
                                        });
                                        break;
                                    }
                                default:
                                    {
                                        result.Add( new RateLimiterTypeInfo
                                        {
                                            Type = RateLimiterType.rlUnknown,
                                            TimeInterval = new System.TimeSpan(0, 0, 1),
                                            RequestLimit = 10
                                        });
                                        break;
                                    }
                            }
                        }
                        break;
                    }
                case Client.BClient:
                    {
                        foreach (var rateLimiterType in rateLimiterTypeList)
                        {
                            switch (rateLimiterType)
                            {
                                case RateLimiterType.rlRequestsPerInterval:
                                    {
                                        result.Add(new RateLimiterTypeInfo
                                        {
                                            ResourceName = AvailableResource.ResourceTwo,
                                            Type = RateLimiterType.rlRequestsPerInterval,
                                            TimeInterval = new System.TimeSpan(0, 0, 1),
                                            RequestLimit = 10
                                        });
                                        result.Add(new RateLimiterTypeInfo
                                        {
                                            ResourceName = AvailableResource.ResourceThree,
                                            Type = RateLimiterType.rlRequestsPerInterval,
                                            TimeInterval = new System.TimeSpan(0, 0, 1),
                                            RequestLimit = 10
                                        });
                                        break;
                                    }
                                case RateLimiterType.rlRequestsPerTimeout:
                                    {
                                        result.Add(new RateLimiterTypeInfo
                                        {
                                            Type = RateLimiterType.rlRequestsPerTimeout,
                                            TimeInterval = new System.TimeSpan(0, 0, 1),
                                            RequestLimit = 10
                                        });
                                        break;
                                    }
                                default:
                                    {
                                        result.Add(new RateLimiterTypeInfo
                                        {
                                            Type = RateLimiterType.rlUnknown,
                                            TimeInterval = new System.TimeSpan(0, 0, 1),
                                            RequestLimit = 10
                                        });
                                        break;
                                    }
                            }
                        }
                        break;
                    }
                case Client.CClient:
                    {
                        foreach (var rateLimiterType in rateLimiterTypeList)
                        {
                            switch (rateLimiterType)
                            {
                                case RateLimiterType.rlRequestsPerInterval:
                                    {
                                        result.Add(new RateLimiterTypeInfo
                                        {
                                            ResourceName = AvailableResource.ResourceTwo,
                                            Type = RateLimiterType.rlRequestsPerInterval,
                                            TimeInterval = new System.TimeSpan(0, 0, 1),
                                            RequestLimit = 10
                                        });
                                        break;
                                    }
                                case RateLimiterType.rlRequestsPerTimeout:
                                    {
                                        result.Add(new RateLimiterTypeInfo
                                        {
                                            ResourceName = AvailableResource.ResourceThree,
                                            Type = RateLimiterType.rlRequestsPerTimeout,
                                            TimeInterval = new System.TimeSpan(0, 0, 1),
                                            RequestLimit = 10
                                        });
                                        break;
                                    }
                                default:
                                    {
                                        result.Add(new RateLimiterTypeInfo
                                        {
                                            Type = RateLimiterType.rlUnknown,
                                            TimeInterval = new System.TimeSpan(0, 0, 1),
                                            RequestLimit = 10
                                        });
                                        break;
                                    }
                            }
                        }
                        break;
                    }
                default: 
                    {
                        result.Add(new RateLimiterTypeInfo
                        {
                            Type = RateLimiterType.rlUnknown,
                            TimeInterval = new System.TimeSpan(0, 10, 0),
                            RequestLimit = 10
                        });
                        break;
                    }
            }
            return result;
        }
    }
}
