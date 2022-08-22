using System;
using System.Linq;
using RateLimiter.DataStore;

namespace RateLimiter.Rules
{
    public class RuleB : IRule
    {
        private int Period { get; set; }

        private readonly IRuleBStore _ruleBStore;

        private RuleB(int period)
        {
            Period = period;
            _ruleBStore = new RuleBStore();
        }

        public static RuleB Configure(int period)
        {
            return new(period);
        }

        public bool Execute(string token)
        {
            var lastRequest = _ruleBStore.GetRuleAByToken(token);
            if (lastRequest == null)
            {
                _ruleBStore.InsertTokenInformation(token, DateTime.Now);
                return true;
            }

            if ((DateTime.Now - lastRequest.LastRequestDateTime).Seconds > Period)
            {
                lastRequest.LastRequestDateTime = DateTime.Now;
                return true;
            }

            return false;
        }
    }
}