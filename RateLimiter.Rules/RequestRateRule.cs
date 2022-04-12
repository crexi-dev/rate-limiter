using RateLimiter.Interfaces;
using RateLimiter.Models;
using RateLimiter.Repository;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RateLimiter.Rules
{
    public class RequestRateRule : IRateLimiter
    {

        private readonly ConcurrentDictionary<string, RequestDetails> _DBStore;
        private const int _MaxRequestCount = 5;
        private readonly IRepository _repository;

        public RequestRateRule()
        {
            _repository = new InMemoryStorage();
            _DBStore = _repository.AccessStorage();
        }


        public bool ApplyRule(string token)
        {

            if (_DBStore.TryGetValue(token, out RequestDetails requestDetails))
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
                return _DBStore.TryAdd(token, request);
            }
        }
    }
}
