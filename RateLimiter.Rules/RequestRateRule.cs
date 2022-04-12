using RateLimiter.Interfaces;
using RateLimiter.Models;
using RateLimiter.Storage;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RateLimiter.Rules
{
    public class RequestRateRule : IRateLimiter
    {

        private readonly ConcurrentDictionary<string, RequestDetails> DBStore;
        private const int _MaxRequestCount = 5;

        public RequestRateRule()
        {
            DBStore = new InMemoryStore().AccessStorage();
        }


        public bool ApplyRule(string token)
        {

            if (DBStore.TryGetValue(token, out RequestDetails requestDetails))
            {

                //if (request.recordedTimeInterval.TotalMinutes > 1)
                //    return false;
                //else if (request.recordedTimeInterval.TotalMinutes < 1 && request.count >= _MaxRequestCount)
                //    return false;
                if (requestDetails.count > _MaxRequestCount)
                {
                    return false;
                }
                else
                {
                    requestDetails.count += 1;
                    return true;
                }

            }
            else
            {
                var request = new RequestDetails
                {
                    count = 1,
                    recordedTimeInterval = new TimeSpan(0, 1, 0)
                };
                return DBStore.TryAdd(token, request);
            }
        }
    }
}
