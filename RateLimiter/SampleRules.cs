using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace RateLimiter
{
    public delegate bool RateLimitRule(string clientId, DateTime timestamp);

    public static class SampleRules
    {
        private static readonly ConcurrentDictionary<string, ClientRequestInfo> _clients = 
            new ConcurrentDictionary<string, ClientRequestInfo>();

        public static RateLimitRule CreateTimeWindowRule(int maxRequests, TimeSpan timeWindow)
        {
            return (clientId, timestamp) =>
            {
                var clientInfo = _clients.GetOrAdd(clientId, _ => new ClientRequestInfo());

                lock (clientInfo.Lock)
                {
                    // Remove expired requests
                    clientInfo.RemoveExpiredRequests(timestamp, timeWindow);

                    // Check if within limit
                    if (clientInfo.RequestTimestamps.Count < maxRequests)
                    {
                        clientInfo.RequestTimestamps.Add(timestamp);
                        return true;
                    }

                    return false;
                }
            };
        }

        public static RateLimitRule CreateDailyQuotaRule(int maxDailyRequests)
        {
            return (clientId, timestamp) =>
            {
                var clientInfo = _clients.GetOrAdd(clientId, _ => new ClientRequestInfo());
                
                lock (clientInfo.Lock)
                {
                    var startOfDay = timestamp.Date;
                    var requestsToday = clientInfo.RequestTimestamps.Where(t => t.Date == startOfDay).Count();
                    
                    if (requestsToday < maxDailyRequests)
                    {
                        clientInfo.RequestTimestamps.Add(timestamp);
                        return true;
                    }
                    
                    return false;
                }
            };
        }
        public static RateLimitRule CreateCertainTimespanPassed(TimeSpan timeWindow)
        {
            return (clientId, timestamp) =>
            {
                var clientInfo = _clients.GetOrAdd(clientId, _ => new ClientRequestInfo());

                lock (clientInfo.Lock)
                {
                    // Remove expired requests
                    if (clientInfo.RequestTimestamps.Count <= 0 || timestamp - clientInfo.RequestTimestamps.Last().ToUniversalTime() >= timeWindow)
                    {
                        clientInfo.RequestTimestamps.Add(timestamp);
                        return true;
                    }

                    return false;
                }
            };
        }
    }

    internal class ClientRequestInfo
    {
        public List<DateTime> RequestTimestamps { get; } = [];
        public object Lock { get; } = new object();

        public void RemoveExpiredRequests(DateTime timestamp, TimeSpan timeWindow)
        {
            while (RequestTimestamps.Count > 0 && timestamp - RequestTimestamps[0] > timeWindow)
            {
                RequestTimestamps.RemoveAt(0);
            }
        }
                    
    }
}