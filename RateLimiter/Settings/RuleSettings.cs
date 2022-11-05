using RateLimiter.RateLimitRules;
using System.Collections.Generic;

namespace RateLimiter.Settings
{

    public class RateLimiterRules
    {
        public List<RulesSetting> RulesSettings { get; set; }
    }
    public class RulesSetting
    {
        public string Key { get; set; }
        public List<Rule> Rules { get; set; }
    }

    public class Rule
    {
        public RulePerRequest RulePerRequest { get; set; }
        public RulePerTimeSpan RulePerTimeSpan { get; set; }
    }



}
