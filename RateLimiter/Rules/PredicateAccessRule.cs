using System;
using System.Collections.Generic;
using System.Linq;

namespace RateLimiter.Rules
{
    public class PredicateAccessRule: AccessRule
    {
        private readonly List<(Func<string, string, bool> predicate, AccessRule accessRule)> _accessRuleConditions;

        public PredicateAccessRule(IEnumerable<(Func<string, string, bool> predicate, AccessRule accessRule)> accessRuleConditions)
        {
            _accessRuleConditions = accessRuleConditions.ToList();
        }

        public override bool Validate(string resourceName, string accessKey)
        {
            var accessRule = _accessRuleConditions.FirstOrDefault(x => x.Item1(resourceName, accessKey)).accessRule;

            if (accessRule == null)
            {
                throw new Exception(
                    $"Predicated wasn't configured for {nameof(resourceName)}: {resourceName}; {nameof(accessKey)}: {accessKey}");
            }

            return accessRule.Validate(resourceName, accessKey);
        }
    }
}
