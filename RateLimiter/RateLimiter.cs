using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter
{
    public class RateLimiter
    {
        private readonly ConcurrentDictionary<string, IRateLimiter> _resourceRules = new();

        public void ConfigureResource(string resource, IRateLimiter rule)
        {
            _resourceRules[resource] = rule;
        }

        public bool AllowRequest(string resource, string clientId)
        {
            return !_resourceRules.ContainsKey(resource) || _resourceRules[resource].AllowRequest(clientId);
        }
    }
}
