using System;
using System.Collections.Generic;

namespace RateLimiter
{
    public class Settings
    {
        public List<RuleDefinition>? Rules { get; set; }
        public Dictionary<string, List<string>>? ApiRuleMapping { get; set; }

        public List<RuleDefinition> GetRulesToApply(string apiName)
        {
            if (this.ApiRuleMapping == null || this.Rules == null) return new List<RuleDefinition>();

            if (this.ApiRuleMapping.TryGetValue(apiName, out List<string>? rulesToApply))
            {
                return this.Rules.FindAll(r => !string.IsNullOrWhiteSpace(r.name) && rulesToApply.Contains(r.name));
            }
            else
            {
                return new List<RuleDefinition>()
                {
                    this.Rules.Find(r => !string.IsNullOrWhiteSpace(r.name) && r.name == "DefaultPlan") ?? throw new ArgumentNullException(nameof(this.Rules))
                };
            }
        }
    }

   public class RuleDefinition
   {
       public string? name { get; set; }
       public int timeSpanSeconds { get; set; }
       public int allowedRequests { get; set; }
   }
}

