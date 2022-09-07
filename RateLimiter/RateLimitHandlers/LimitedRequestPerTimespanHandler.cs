using Core.Models;
using Microsoft.Extensions.Caching.Memory;
using System;

namespace RateLimiter
{
    public class LimitedRequestPerTimespanHandler 
        : ClientDataProvider, IRateLimitHandler
    {
        private readonly IMemoryCache cache_;

        public LimitedRequestPerTimespanHandler(IMemoryCache cache)
            :base(cache)
        {
            cache_ = cache;
        }

        public bool IsRateLimitSucceded(RateLimitDecorator decorator, string key)
        {
            ClientData clientData = GetClientDataByKey(key);
            if (clientData != null)
            {
                return DateTime.UtcNow < clientData.PreviousSuccessfullCallTime.AddSeconds(decorator.TimeSpan) &&
                    clientData.NumberOfRequestsCompletedSuccessfully == decorator.MaxRequests;
            }
            return false;
        }

        public void UpdateClientData(RateLimitDecorator decorator, string key)
        {
            ClientData clientData = GetClientDataByKey(key);

            if (clientData != null)
            {
                clientData.PreviousSuccessfullCallTime = DateTime.UtcNow;

                if (clientData.NumberOfRequestsCompletedSuccessfully == decorator.MaxRequests)
                {
                    clientData.NumberOfRequestsCompletedSuccessfully = 1;
                }

                else
                {
                    clientData.NumberOfRequestsCompletedSuccessfully++;
                }
            }
            else
            {
                clientData = new ClientData(DateTime.UtcNow, 1);
            }
            cache_.Set(key, clientData);
        }
    }
}
