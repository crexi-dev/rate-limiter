using RateLimiter.Models.Conditions;
using RateLimiter.Models.Configuration;
using RateLimiter.Models.Policies;
using System.Collections.Generic;
using System.Linq;

namespace RateLimiter.Factories
{
    public class ResourcePoliciesFactory
    {
        private readonly RuleFactory ruleFactory;
        private readonly ConditionFactory conditionFactory;

        public ResourcePoliciesFactory()
        {
            ruleFactory = new RuleFactory();
            conditionFactory = new ConditionFactory();
        }

        public IEnumerable<ResourcePolicies> Create(IRateLimiterConfiguration configuration)
        {
            return configuration.ResourceConfigurations
                .Select(resourceConfiguration => new ResourcePolicies
                {
                    ResourceName = resourceConfiguration.ResourceName,
                    Policies = CreatePolicies(resourceConfiguration.Policies)
                })
                .ToList();
        }

        private IEnumerable<IPolicy> CreatePolicies(IEnumerable<PolicyConfiguration> policies)
        {
            return policies
                .Select(policy =>
                {
                    var rule = ruleFactory.Create(policy.Rule);
                    var conditions = policy.Conditions?.Select(condition => conditionFactory.Create(condition)).ToList();

                    return new Policy(rule, conditions);
                })
                .ToList();
        }
    }
}
