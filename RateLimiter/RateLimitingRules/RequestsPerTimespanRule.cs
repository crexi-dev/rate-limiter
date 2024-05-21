using RateLimiter.Nugget;
using RateLimiter.Nugget.Interfaces;
using System;
using System.Collections.Generic;

namespace RateLimiter.RateLimitingRules
{
    public class RequestsPerTimespanRule : IRateLimitRule<RequestsPerTimespanRule>
    {
        public RequestsPerTimespanRule()
        {

        }

        public bool IsRateLimitExceeded(string accesToken)
        {
            if (string.IsNullOrWhiteSpace(accesToken))
                return false;

            SharedRoutes.AddRequestInfo(accesToken, DateTime.Now);
            var _requests = SharedRoutes.GetRequestInfo(accesToken);

            _requests.RemoveAll(ts => ts < DateTime.Now.AddSeconds(-10));

            if (_requests.Count > 3)
            {
                return true;
            }

            return false;
        }

        public List<string> GetRoutes() => new() { "/weatherforecast".ToLower() };
    }
}
