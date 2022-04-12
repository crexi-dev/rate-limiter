using RateLimiter.Interfaces;
using RateLimiter.Models;
using RateLimiter.Repository;
using System;
using System.Collections.Concurrent;

namespace RateLimiter.Rules
{
    public class LastCallRule : IRateLimiter
    {

        private readonly ConcurrentDictionary<string, RequestDetails> _requests;
        private const int _MaxAllowedMinutes = 1;
        private readonly IRepository _repository;

        public LastCallRule()
        {

            _repository = new InMemoryStorage();
            _requests = _repository.AccessStorage();
        }

        public bool ApplyRule(string token)
        {
            bool ruleStatus = false;
            if (_requests.TryGetValue(token, out RequestDetails requestDetails))
            {
                if (requestDetails.TimeRequestMade.HasValue)
                {
                    if ((DateTime.UtcNow - requestDetails.TimeRequestMade.Value).TotalMinutes > _MaxAllowedMinutes)
                        ruleStatus = false;
                    else
                    {
                        requestDetails.TimeRequestMade = DateTime.UtcNow;
                        ruleStatus = true;
                    }
                }
            }
            else
            {
                var request = new RequestDetails
                {
                    TimeRequestMade = DateTime.UtcNow
                };
                ruleStatus = _requests.TryAdd(token, request);
            }
            return ruleStatus;
        }
    }
}
