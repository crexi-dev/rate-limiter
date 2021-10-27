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
        // Token : VisitLimiter
        //public Dictionary<string, VisitLimiter> visitLimiters;

        private ConcurrentDictionary<(string Resource, string Region), ResourceLimiter> _resourceLimiters;

        public ApiLimiter(ResourceRules resourceRules)
        {
            //visitLimiters = new Dictionary<string, VisitLimiter>();
            _resourceLimiters = new ConcurrentDictionary<(string Resource, string Region), ResourceLimiter>();
            foreach (var resource in resourceRules.ResourceRuleList)
            {
                var resourceLimiter = new ResourceLimiter();
                resourceLimiter.AddRules(resource.Rules);
                _resourceLimiters.TryAdd((resource.ResourceName, resource.Region), resourceLimiter);
            }
        }

        public bool Verify(string resource, string token)
        {
            //if (!_resourceLimiters.ContainsKey((resource, region)))
            //    return true;

            if (!_resourceLimiters.TryGetValue((resource, region), out var resourceLimiter))
                return true;

            return resourceLimiter.Verify(token);
            //_resourceLimiters.GetOrAdd((resource, region), (data, rl) =>
            //{

            //});
        }
    }
}
