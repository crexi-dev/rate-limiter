using System.Collections.Generic;
using RateLimiter.Configurators;
using RateLimiter.RuleEngines;
using RateLimiter.Rules;

namespace RateLimiter
{
    public class LimitedService
    {
        private const int Period = 1;
        private const int AllowedRequests = 5;

        private readonly RuleEngine _ruleEngine1;
        private readonly RuleEngine _ruleEngine2;

        public LimitedService()
        {
            // Sample rule instances
            var ruleA = RuleAConfigurator.GetRule(Period, AllowedRequests);
            var ruleB = RuleBConfigurator.GetRule(Period);
            
            // for demonstration purpose, there are more than one rule engine since we don't have IOC container to test.
            // for real world scenarios, where there is an IOC container present, no need to do the following.
            _ruleEngine1 = RuleEngine1Configurator.GetRuleEngine(new List<IRule> {ruleA});
            _ruleEngine2 = RuleEngine2Configurator.GetRuleEngine(new List<IRule> {ruleA, ruleB});
        }

        public void SampleServiceOne(string token)
        {
            _ruleEngine1.Run(token);
        }


        public void SampleServiceTwo(string token)
        {
            _ruleEngine2.Run(token);
        }
    }
}