using System;
using System.Collections.Generic;
using RateLimiter.RulesEngine.Library;
using RateLimiter.RulesEngine.Library.Rules;

namespace RateLimiter.RulesEngine.Library.Repository
{
    public class RuleRepository : IRuleRepository {
        private Dictionary<int, Rule> rules;
        private int ruleCount;
        private Dictionary<RateLimitLevel, Dictionary<RateLimitType, RateLimitSettings>> rateLimitSettings;

        public RuleRepository() {
            // read rule data from db, file, etc.
            this.ruleCount = 0;
            rules = new Dictionary<int, Rule>();
            rateLimitSettings = new Dictionary<RateLimitLevel, Dictionary<RateLimitType, RateLimitSettings>>();
            this.InitializeFakeRepository();
        }

        public int AddRule(Rule rule) {
            this.ruleCount += 1;
            rules.Add(this.ruleCount, rule);
            return ruleCount;
        }

        public Rule GetRule(string resource, string serverIP) {
            return null;
        }

        private void InitializeFakeRepository() {
            // create rules
            var ruleUS = new Rule("US", RateLimitType.RequestsPerTimespan, RateLimitLevel.Default);
            var ruleEU = new Rule("EU", RateLimitType.TimespanPassedSinceLastCall, RateLimitLevel.Default);
            var ruleCombo = new Rule("Combo rule", RateLimitType.RequestsPerTimespan | RateLimitType.TimespanPassedSinceLastCall, RateLimitLevel.Default);

            rules[2] = ruleUS;
            rules[3] = ruleEU;
            rules[4] = ruleCombo;
        }
    }
}