using RateLimiterMy.Contracts;
using RateLimiterMy.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiterMy.RateRules
{
    public class RuleManager : IRuleManager
    {
        private ConcurrentDictionary<string, ICollection<IRule>> ResourcesRules = new ConcurrentDictionary<string, ICollection<IRule>>();
        private ConcurrentDictionary<Location, ICollection<IRule>> LocationRules = new ConcurrentDictionary<Location, ICollection<IRule>>();

        public void AddResourcesRule(string resourcesName, IRule rule)
        {
            if (resourcesName == null) throw new ArgumentNullException(nameof(resourcesName));
            if (rule == null) throw new ArgumentNullException(nameof(rule));

            if (ResourcesRules.TryGetValue(resourcesName, out var existingRules))
            {
                existingRules ??= new List<IRule>();
                existingRules.Add(rule);
            }
            else ResourcesRules[resourcesName] = new List<IRule>() { rule };
        }

        public void AddRegionRule(Location local, IRule rule)
        {
            if (local == 0) throw new ArgumentException(nameof(local));
            if (rule == null) throw new ArgumentNullException(nameof(rule));

            if (LocationRules.TryGetValue(local, out var existingRule))
            {
                existingRule ??= new List<IRule>();
                existingRule.Add(rule);
            }
            else LocationRules[local] = new List<IRule>() { rule };
        }

        public ICollection<IRule> GetCurrentRules(IRequest request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));//

            var result = new List<IRule>();

            LocationRules.TryGetValue(request.Location, out var rulesLocation);
            if(rulesLocation != null) result.AddRange(rulesLocation);

            ResourcesRules.TryGetValue(request.Controler, out var rulesResources);
            if (rulesResources != null) result.AddRange(rulesResources);

            return result;
        }

        public bool Validate(IRequest request) => Validate(GetCurrentRules(request), request);

        private bool Validate(ICollection<IRule> rules, IRequest request)
        {
            bool requestConforms = true;

            foreach (var rule in rules)
            {
                if (!rule.Validate(request)) requestConforms = false;
            }

            return requestConforms;
        }

    }
}
