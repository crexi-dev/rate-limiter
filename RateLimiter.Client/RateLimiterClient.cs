using System;

namespace RateLimiter.Client
{
    public class RateLimiterClient : IRateLimiterClient {
        private IRateLimiterProxy rateLimiterProxy;

        public RateLimiterClient(IRateLimiterProxy rateLimiterProxy) {
            this.rateLimiterProxy = rateLimiterProxy;
        }

        public bool Verify(string token, DateTime requestDate, string serverIP) {
            return this.rateLimiterProxy.Verify(token, requestDate, serverIP);
        }
    }
}