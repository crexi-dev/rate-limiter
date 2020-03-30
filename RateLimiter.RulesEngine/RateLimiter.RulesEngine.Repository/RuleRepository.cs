using System;
using System.Collections.Generic;
using RateLimiter.RulesEngine;

namespace RateLimiter.RulesEngine.Repository
{
    public class RuleRepository : IRuleRepository {
        private Dictionary<int, Rule> rules;
        private Dictionary<RateLimitLevel, Dictionary<RateLimitType, RateLimitSettings>> rateLimitSettings;
        private const int TOKEN_BUCKET_MAX_AMOUNT_DEFAULT = 5;
        private const int TOKEN_BUCKET_REFILL_TIME_DEFAULT = 60;
        private const int TOKEN_BUCKET_REFILL_AMOUNT_DEFAULT = 5;
        private const int TIMESPAN_PASSED_MINUTES_DEFAULT = 1;

        public RuleRepository() {
            // read rule data from db, file, etc.
            rules = new Dictionary<int, Rule>();
            rateLimitSettings = new Dictionary<RateLimitLevel, Dictionary<RateLimitType, RateLimitSettings>>();
            this.InitializeFakeRepository();
        }

        public int CreateRule(Rule rule) {
            return 1;
        }

        public IEnumerable<Rule> GetRules(string serverIP) {

            return null;
        }

        private void InitializeFakeRepository() {
            // create rules
            var ruleUS = new RegionRule(2, "US", RateLimitType.RequestsPerTimespan, RateLimitLevel.Default);
            var ruleEU = new RegionRule(3, "EU", RateLimitType.TimespanPassedSinceLastCall, RateLimitLevel.Default);
            var ruleCombo = new RegionRule(4, "Combo rule", RateLimitType.RequestsPerTimespan | RateLimitType.TimespanPassedSinceLastCall, RateLimitLevel.Default);

            rules[2] = ruleUS;
            rules[3] = ruleEU;
            rules[4] = ruleCombo;

            var defaultSettings = new Dictionary<RateLimitType, RateLimitSettings>();

            var requestsPerTimespanSettings = new RequestsPerTimespanSettings() {
                MaxAmount = TOKEN_BUCKET_MAX_AMOUNT_DEFAULT,
                RefillAmount = TOKEN_BUCKET_REFILL_AMOUNT_DEFAULT,
                RefillTime = TOKEN_BUCKET_REFILL_TIME_DEFAULT
            };

            var timespanPassedSinceLastCallSettings = new TimespanPassedSinceLastCallSettings() {
                TimespanLimit = new TimeSpan(0, TIMESPAN_PASSED_MINUTES_DEFAULT, 0)
            };

            defaultSettings[RateLimitType.RequestsPerTimespan] = requestsPerTimespanSettings;
            defaultSettings[RateLimitType.TimespanPassedSinceLastCall] = timespanPassedSinceLastCallSettings;

            rateLimitSettings[RateLimitLevel.Default] = defaultSettings;
        }
    }
}