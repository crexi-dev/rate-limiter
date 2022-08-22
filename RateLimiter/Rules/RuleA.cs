using System;
using System.Linq;
using RateLimiter.DataStore;

namespace RateLimiter.Rules
{
    public class RuleA : IRule
    {
        private int Period { get; }
        private int RequestsPerPeriod { get; }

        private RuleA(int period, int requestsPerPeriod)
        {
            Period = period;
            RequestsPerPeriod = requestsPerPeriod;
        }
        
        public static RuleA Configure(int period, int requestsPerPeriod)
        {
            return new RuleA(period, requestsPerPeriod);
        }

        public bool Execute(string token)
        {
            var lastRequest = DataStore.DataStore.RuleAStores.FirstOrDefault(x => x.Token == token);

            if (lastRequest == null)
            {
                var ruleData = new RuleAStore
                {
                    RequestTimeSpan = DateTime.Now,
                    RemainingRequestsInTimeSpan = RequestsPerPeriod,
                    Token = token
                };
                DataStore.DataStore.RuleAStores.Add(ruleData);
                return true;
            }

            if (lastRequest.RemainingRequestsInTimeSpan == 1 &&
                (DateTime.Now - lastRequest.RequestTimeSpan).Seconds < Period)
            {
                return false;
            }

            lastRequest.RemainingRequestsInTimeSpan--;
            return true;
        }
    }
}