using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using RateLimiter.Data;
using RateLimiter.Model;
using RateLimiter.Rule;

[assembly: InternalsVisibleTo("RateLimiter.Tests")]

namespace RateLimiter
{
    /// <summary>
    ///     This design is for demo purposes.  
    ///     Register this as singleton.  
    ///     Else we can write this as configuration builder with fluent syntax.
    ///     Something similar to this https://learn.microsoft.com/en-us/aspnet/core/performance/rate-limit?view=aspnetcore-8.0
    /// </summary>
    internal class RateLimitService : IRateLimitService
    {
        private readonly IRequestTrafficDataAccess requestTrafficDataAccess;
        private readonly ConcurrentDictionary<string, List<RateLimitRuleBase>> resourceRules;
        
        public RateLimitService(IRequestTrafficDataAccess requestTrafficDataAccess)
        {
            this.requestTrafficDataAccess = requestTrafficDataAccess;
            resourceRules = new ConcurrentDictionary<string, List<RateLimitRuleBase>>();
        }

        public void AddRule<T>(Uri uri, T rule) where T : RateLimitRuleBase
        {
            if (!resourceRules.TryGetValue(uri.AbsolutePath, out var rules))
            {
                resourceRules.TryAdd(uri.AbsolutePath, new List<RateLimitRuleBase> { rule });
            }
            else
            {
                rules.Add(rule);
            }
        }

        public IEnumerable<RateLimitRuleBase> GetRules(Uri uri)
        {
            return resourceRules.TryGetValue(uri.AbsolutePath, out var rules)
                ? rules
                : Enumerable.Empty<RateLimitRuleBase>();                
        }

        public void ClearRules(Uri uri)
        {
            if (resourceRules.TryGetValue(uri.AbsolutePath, out var rules))
            {
                rules.Clear();
            }
        }

        public bool RequestAllow(RateLimitRequest request)
        {
            // Assume that we get data from some large storage somewhere indexed
            // and filtered by token or session ID from token and URL.
            var requests = requestTrafficDataAccess.GetRequests(request.Token, request.Url);

            if (!resourceRules.TryGetValue(request.Url.AbsolutePath, out var rules))
            {
                return true;
            }

            requests.Append(request);

            return rules.All(r => r.CheckRequestAllow(requests));
        }
    }
}
