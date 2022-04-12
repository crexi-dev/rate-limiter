using RateLimiter.Interfaces;
using RateLimiter.Models;
using RateLimiter.Repository;
using System;
using System.Collections.Concurrent;

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
            bool ruleStatus = false;
            if (_DBStore.TryGetValue(token, out RequestDetails requestDetails))
            {

                if (requestDetails.count > _MaxRequestCount)
                    ruleStatus =  false;
                
                else
                {
                    requestDetails.count++;
                    ruleStatus =  true;
                }

            }
            else
            {
                var request = new RequestDetails
                {
                    count = 1,
                    recordedTimeInterval = new TimeSpan(0, 1, 0),
                    TimeRequestMade = DateTime.UtcNow
                    
                };
                ruleStatus =  _DBStore.TryAdd(token, request);
            }
            return ruleStatus;
        }
    }
}
