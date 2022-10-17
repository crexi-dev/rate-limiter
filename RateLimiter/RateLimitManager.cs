using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Runtime.Caching;
using System.Linq;

namespace RateLimiter
{
    public class RateLimitManager
    {
        //public List<Client> Clients { get; set; } = new List<Client>();
        public List<ClientRules> Rules { get; set; } = new List<ClientRules>();

        private Dictionary<string, LimitCounter> clientCounters = new Dictionary<string, LimitCounter>();

        public struct LimitCounter
        {
            public DateTime Timestamp { get; set; }

            public double Count { get; set; }
        }

        public RateLimitManager(List<ClientRules> rules) 
        { 
            //Clients = clients;
            Rules = rules;
        }

        public RateLimitManager() { }

        public bool IsAllowed(string clientId) 
        {
            ClientRules cr = Rules.Where(r => r.ClientId == clientId).FirstOrDefault();
            if (cr != null && cr.Rules != null) 
            {
                foreach (LimitRule rule in cr.Rules)
                {
                    if (clientCounters.ContainsKey(clientId))
                    {
                        LimitCounter counter = (LimitCounter)clientCounters.GetValueOrDefault(clientId);
                        if (counter.Timestamp + rule.PeriodTimespan.Value < DateTime.UtcNow || counter.Count < rule.MaxRequests)
                        {
                            counter.Timestamp = DateTime.UtcNow;
                            counter.Count++;
                            clientCounters[clientId] = counter;
                            continue;
                        }
                        if (rule.WaitPeriod || counter.Count >= rule.MaxRequests)
                        {
                            return false;
                        }
                    }
                    else
                    {
                        LimitCounter counter = new LimitCounter();
                        counter.Timestamp = DateTime.UtcNow;
                        counter.Count = 1;
                        clientCounters.Add(clientId, counter);
                    }
                }
            }
            return true;
        }
    }

    public class LimitRule 
    {
        public string Period { get; set; }

        public TimeSpan? PeriodTimespan { get; set; }

        public int MaxRequests { get; set; }

        public string Endpoint { get; set; } 

        public bool WaitPeriod { get; set; }

    }

    public class ClientRules 
    {
        public string ClientId { get; set; }
        public List<LimitRule> Rules { get; set; } = new List<LimitRule>();

        public ClientRules(string clientId, List<LimitRule> rules)
        {
            ClientId = clientId;
            Rules = rules;
        }
    }
}
