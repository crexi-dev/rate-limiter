using RateLimiter.Domain.Resource;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter.Domain.ApiLimiter
{
    public class ApiLimiter : IApiLimiter
    {
        public const string GLOBAL = "GLOBAL";

        private ConcurrentDictionary<(string Resource, string Region), TokenBucket> _resourceLimiters;

        public ApiLimiter(ResourceRules resourceRules)
        {
            //visitLimiters = new Dictionary<string, VisitLimiter>();
            _resourceLimiters = new ConcurrentDictionary<(string Resource, string Region), TokenBucket>();
            foreach (var resource in resourceRules.ResourceRuleList)
            {
                var tokenBucket = new TokenBucket();
                tokenBucket.AddRules(resource.Rules);
                _resourceLimiters.TryAdd((resource.ResourceName, resource.Region), tokenBucket);
            }
        }

        public bool Verify(string resource, string token)
        {
            // Assume the region is embedded with the token
            // TODO : Change this logic. Perhaps embed the region inside a JWT token?
            string region = token.Substring(0, 2);

            bool result = true;
            bool globalResult = true;

            TokenBucket globalResourceTokenBucket = null;
            if (!_resourceLimiters.TryGetValue((resource, region), out var tokenBucket) &&
                !_resourceLimiters.TryGetValue((resource, GLOBAL), out globalResourceTokenBucket))
                return true;

            if (tokenBucket != null)
                result = tokenBucket.Verify(token);

            if (globalResourceTokenBucket != null)
                globalResult = globalResourceTokenBucket.Verify(token);

            return result && globalResult;
        }
    }
}
