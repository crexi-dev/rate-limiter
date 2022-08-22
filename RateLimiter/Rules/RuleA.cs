using System;
using System.Linq;
using RateLimiter.DataStore;

namespace RateLimiter.Rules
{
    public class RuleA : IRule
    {
        private int Period { get; }
        private int RequestsPerPeriod { get; }

        private readonly IRuleAStore _ruleAStore;
        
        private RuleA(int period, int requestsPerPeriod)
        {
            Period = period;
            RequestsPerPeriod = requestsPerPeriod;
            _ruleAStore = new RuleAStore();
        }
        
        public static RuleA Configure(int period, int requestsPerPeriod)
        {
            return new RuleA(period, requestsPerPeriod);
        }

        public bool Execute(string token)
        {
            var lastRequest = _ruleAStore.GetRuleAByToken(token);

            if (lastRequest == null)
            {
                _ruleAStore.InsertTokenInformation(token, DateTime.Now, RequestsPerPeriod);
                return true;
            }

            if (lastRequest.RemainingRequests == 1 &&
                (DateTime.Now - lastRequest.LastRequestDateTime).Seconds < Period)
            {
                return false;
            }

            lastRequest.RemainingRequests--;
            return true;
        }
    }
}