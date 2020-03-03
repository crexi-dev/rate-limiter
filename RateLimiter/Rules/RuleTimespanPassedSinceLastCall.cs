using RuleEngine;
using System;
using RateLimiter.Model;

namespace RateLimiter.Rules
{
    public abstract class RuleTimespanPassedSinceLastCall : IRule
    {
        public string Name => "RuleTimespanPassedSinceLastCall";

        protected abstract TimeSpan WaitTime { get; }

        public void Execute(IRuleEnvironment environment)
        {
            var result = environment.GetFact<RuleResult>("result");
            if (!result.Allow)
                return;

            TokenInfo tokenInfo = environment.GetFact<TokenInfo>("tokenInfo");

            if (DateTime.Now < tokenInfo.LastRequestTime.Add(WaitTime))
            {
                result.Allow = false;
                result.Message = $"A certain timespan {WaitTime} should pass since last call.";
            }
        }
    }
}
