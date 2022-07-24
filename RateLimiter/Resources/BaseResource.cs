using RuleLimiterTask.Rules;

namespace RuleLimiterTask.Resources
{
    public class BaseResource
    {
        private readonly IList<IRule> _rules = new List<IRule>();

        public void ApplyRule(IRule rule)
        {
            _rules.Add(rule);
        }

        public bool CheckAccess(UserRequest request, ICacheService cache)
        {
            //return !_rules.Any(x => !x.IsValid(request, cache));

            foreach (var rule in _rules) 
            {
                if(!rule.IsValid(request, cache))
                    return false;
            }

            return true;
        }
    }
}
