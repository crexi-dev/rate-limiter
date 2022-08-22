using System.Collections.Generic;
using RateLimiter.Configurators;
using RateLimiter.RuleEngines;
using RateLimiter.Rules;

namespace RateLimiter
{
    public class LimitedService
    {
        // These numbers can be defined in a configuration setting file
        private const int Period = 10;
        private const int AllowedRequests = 5;

        private const int UsBasedPeriod = 10;
        private const int UsBasedAllowedNumberOfRequests = 3;
        private const int EuBasedPeriod = 5;

        private readonly RuleEngine _ruleEngine1;
        private readonly RuleEngine _ruleEngine2;
        private readonly RuleEngine _ruleEngine3;

        public LimitedService()
        {
            // Sample rule instances
            var ruleA = RuleAConfigurator.GetRule(Period, AllowedRequests);
            var ruleB = RuleBConfigurator.GetRule(Period);
            var ruleC = RuleCConfigurator.GetRule(UsBasedPeriod, UsBasedAllowedNumberOfRequests, EuBasedPeriod);

            // for demonstration purpose, there are more than one rule engine since we don't have IOC container to test.
            // for real world scenarios, where there is an IOC container present, no need to do the following.
            // the code inside rule engine configurators are similar but it's just for testing purposes and didn't want to make things complicated by creating Generic
            _ruleEngine1 = RuleEngine1Configurator.GetRuleEngine(new List<IRule> {ruleA});
            _ruleEngine2 = RuleEngine2Configurator.GetRuleEngine(new List<IRule> {ruleB});
            _ruleEngine3 = RuleEngine3Configurator.GetRuleEngine(new List<IRule> {ruleC});
        }

        public void SampleServiceOne(string token)
        {
            _ruleEngine1.Run(token);
        }
        
        public void SampleServiceTwo(string token)
        {
            _ruleEngine2.Run(token);
        }

        public void SampleServiceThree(string token)
        {
            _ruleEngine3.Run(token);
        }
    }
}