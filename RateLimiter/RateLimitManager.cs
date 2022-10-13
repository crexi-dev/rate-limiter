using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Runtime.Caching;
using System.Linq;

namespace RateLimiter
{
    internal class RateLimitManager
    {
        public List<Client> Clients { get; set; } = new List<Client>();
        public List<ClientPolicies> Policies { get; set; } = new List<ClientPolicies>();

        private readonly MemoryCache _cache = MemoryCache.Default;

        public struct LimitCounter
        {
            public DateTime Timestamp { get; set; }

            public double Count { get; set; }
        }

        public RateLimitManager(List<Client> clients, List<ClientPolicies> policies) 
        { 
            Clients = clients;
            Policies = policies;
        }

        public bool IsAllowed(string clientId, int requestCount) 
        {
            ClientPolicies cp = Policies.Where(p => p.ClientId == clientId).FirstOrDefault();
            foreach (LimitPolicy policy in cp.Policies.ClientRules) 
            {
                foreach (LimitRule rule in policy.Rules) 
                {
                    if (rule.MaxRequests > 0)
                    {
                        LimitCounter counter = (LimitCounter)_cache.Get(clientId);
                        if (counter.Timestamp + rule.PeriodTimespan.Value < DateTime.UtcNow)
                        {
                            counter.Timestamp = DateTime.UtcNow;
                            continue;
                        }
                        if (counter.Count > rule.MaxRequests)
                        {
                            return false;
                        }
                    }
                    else 
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }

    internal class LimitOptions
    {
        public int StatusCode { get; set; } = 429;
        public string Message { get; set; }
    }

    internal class Client 
    { 
        public string AccessToken { get; set; }
        public string Name { get; set; }
        public string ClientId { get; set; }
    }

    internal class LimitRule 
    {
        public string Period { get; set; }

        public TimeSpan? PeriodTimespan { get; set; }

        public double MaxRequests { get; set; }

    }

    internal class LimitPolicy 
    {
        public List<LimitRule> Rules { get; set; } = new List<LimitRule>();
    }

    internal class LimitPolicies 
    {
        public List<LimitPolicy> ClientRules { get; set; }
    }

    internal class ClientPolicies 
    {
        public string ClientId { get; set; }
        public LimitPolicies Policies { get; set; }
    }
}
