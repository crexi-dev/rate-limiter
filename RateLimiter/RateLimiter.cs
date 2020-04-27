using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter
{
    public class RateLimiter
    {
        private static readonly Lazy<RateLimiter> instance =  new Lazy<RateLimiter>(() => new RateLimiter());

        public static RateLimiter Instance { get { return instance.Value; } }

        public List<RateLimiterRule> Rules { get; private set; }

      
        private RateLimiter() 
        {
            // TODO: Load default rules from the settings (config file / DB / cache) 
           
        }


        public bool ValidateRuleList(int userId, int requestId, List<RateLimiterRule> rules, List<IRateLimiterFilter> targetFilters, EnumFilterMatchMode filterMatchMode = EnumFilterMatchMode.Any)
        {
            if (rules == null)
                return true;

            foreach (var rule in rules)
                return ValidateRule(userId, requestId, rule, targetFilters, filterMatchMode);

            return false;
        }

        public bool ValidatedRuleList(int userId, int requestId, List<RateLimiterRule> rules, IRateLimiterFilter targetFilter = null)
        {
            if (rules == null)
                return true;
            List<IRateLimiterFilter> targetFilters = targetFilter != null ? new List<IRateLimiterFilter>() { targetFilter } : null;
            foreach (var rule in rules)
            {
                if (ValidateRule(userId, requestId, rule, targetFilters))
                    return true;
            }
            return false;
        }

        public bool ValidateRule(int userId,int requestId, RateLimiterRule rule,  List<IRateLimiterFilter> targetFilters, EnumFilterMatchMode filterMatchMode = EnumFilterMatchMode.Any)
        {
            if (rule == null)
                return true;

            if (rule.FiltersMatched(targetFilters, filterMatchMode))
                    return rule.Strategy.IsAllowed(userId,requestId);
            
            return false;
        }


        public bool ValidateRule(int userId, int requestId, RateLimiterRule rule, IRateLimiterFilter targetFilter = null)
        {
            if (rule == null)
                return true;
            List<IRateLimiterFilter> targetFilters = targetFilter != null ? new List<IRateLimiterFilter>() { targetFilter } : null;
            if (rule.FiltersMatched(targetFilters))
                return rule.Strategy.IsAllowed(userId,requestId);

            return false;
        }


    }
}
