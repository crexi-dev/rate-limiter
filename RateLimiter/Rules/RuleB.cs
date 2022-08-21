using System;
using System.Linq;

namespace RateLimiter.Rules
{
    public class RuleB : IRule
    {
        private int Period { get; set; }

        private RuleB(int period)
        {
            Period = period;
        }

        public static RuleB Configure(int period)
        {
            return new(period);
        }

        public bool Execute(string token)
        {
            var lastRequest = DataStore.DataStore.RuleBStores.SingleOrDefault(x => x.Token == token);
            if (lastRequest == null)
            {
                var request = new DataStore.RuleBStore()
                {
                    LastRequestTimeSpan = DateTime.Now,
                    Token = token
                };
                DataStore.DataStore.RuleBStores.Add(request);
                return true;
            }

            if ((DateTime.Now - lastRequest.LastRequestTimeSpan).Minutes > Period)
            {
                lastRequest.LastRequestTimeSpan = DateTime.Now;
            }

            return false;
        }
    }
}