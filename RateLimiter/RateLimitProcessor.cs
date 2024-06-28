using RateLimiter.RateLimiter;
using RateLimiter.RateLimiter.Models;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace RateLimiter
{
    public class RateLimitProcessor
    {
        // (policy,region) - limiter
        private readonly Dictionary<(string, Region), ILimiter> _registeredLimitersByPolicy;

        // (token, resource) - request
        private readonly ConcurrentDictionary<(string, string), ClientRequest> _requestData;

        private object _lock = new object();

        public RateLimitProcessor(Dictionary<(string, Region), ILimiter> registeredLimiters)
        {
            _registeredLimitersByPolicy = registeredLimiters;
            _requestData = new ConcurrentDictionary<(string, string), ClientRequest>();
        }

        public LimitResult VerifyRequest(string token, Region region, string resource, string[] policies)
        {
            LimitResult result = new();

            lock (_lock)
            {
                var clientRequest = _requestData.GetOrAdd((token, resource), x => ClientRequest.Empty);

                foreach (var policy in policies)
                {
                    if (!_registeredLimitersByPolicy.TryGetValue((policy, region), out var limiter))
                    {
                        continue;
                    }

                    result = limiter.CheckLimit(clientRequest);

                    if (result.Limited is true)
                        return result;
                }
            }

            return result;
        }
    }
}
