using System;
using RateLimiter.Library;

namespace RateLimiter.Client
{
    public class RateLimiterClient : IRateLimiterClient {
        private IRateLimiterProxy rateLimiterProxy;

        public RateLimiterClient(IRateLimiterProxy rateLimiterProxy) {
            this.rateLimiterProxy = rateLimiterProxy;
        }

        public bool Verify(string token, DateTime requestDate, RateLimitSettingsConfig rateLimitSettings = null)
        {
            return this.rateLimiterProxy.Verify(token, requestDate, rateLimitSettings);
        }
    }
}