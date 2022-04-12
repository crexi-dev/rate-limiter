using RateLimiter.Interfaces;
using RateLimiter.Models;
using System;
using System.Collections.Concurrent;
using RateLimiter.Storage;

namespace RateLimiter.Rules
{
    internal class LastCallRule : IRateLimiter
    {

        private readonly ConcurrentDictionary<string, RequestDetails> _requests;
        private const int _MaxAllowedMinutes = 30;

        public LastCallRule()
        {
            InMemoryStore dbStore = new InMemoryStore();
            _requests = dbStore.AccessStorage();
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
