using Core.Models;
using Microsoft.Extensions.Caching.Memory;
using System;

namespace RateLimiter
{
    public class CertainTimespanPassedSinceLastCallHandler 
        : ClientDataProvider, IRateLimitHandler
    {
        private readonly IMemoryCache cache_;

        public CertainTimespanPassedSinceLastCallHandler(IMemoryCache cache)
            :base(cache)
        {
            cache_ = cache;
        }

        public bool IsRateLimitSucceded(RateLimitDecorator decorator, string key)
        {
            ClientData clientData = GetClientDataByKey(key);
            if (clientData != null)
            {
                return DateTime.UtcNow < clientData.PreviousSuccessfullCallTime.AddSeconds(decorator.TimeSpan);
            }
            return false;
        }

        public void UpdateClientData(RateLimitDecorator decorator, string key)
        {
            ClientData clientData = GetClientDataByKey(key);
            if (clientData != null)
            {
                clientData.PreviousSuccessfullCallTime = DateTime.UtcNow;
            }
            else
            {
                clientData = new ClientData(DateTime.UtcNow);
            }
            cache_.Set(key, clientData);
        }
    }
}