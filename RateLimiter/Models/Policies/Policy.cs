using RateLimiter.Models.Conditions;
using RateLimiter.Models.Rules;
using System.Collections.Generic;
using System.Linq;

namespace RateLimiter.Models.Policies
{
    public sealed class Policy : IPolicy
    {
        private IRule rule;
        private IEnumerable<ICondition> conditions;

        public Policy(IRule rule, IEnumerable<ICondition> conditions)
        {
            this.rule = rule;
            this.conditions = conditions;
        }

        public PolicyResult Execute(СlientStatistics сlientStatistics, IContext context)
        {
            var policyResult = new PolicyResult();

            if (conditions == null || conditions.All(condition => condition.IsMatch(context)))
            {
                var ruleResult = rule.Execute(сlientStatistics);

                if (ruleResult.IsFailed == true)
                    policyResult.Fail(ruleResult.Message);
            }

            return policyResult;
        }
    }
}
