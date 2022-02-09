using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using RateLimiter.Enums;
using RateLimiter.Models;

namespace RateLimiter
{
    public class RulesManager
    {
        private readonly List<RuleModel> _rules;

        public RulesManager()
        {
            _rules = new List<RuleModel>();
            
            ReadJson();
        }
        
        public bool CheckRules(RequestModel requestModel, ref List<RequestModel> acceptedRequests, bool bothRules)
        {
            var applyingRule = GetLocation(requestModel.Token);
            if (applyingRule == null) return false;
            
            var regionRequests = GetCurrentRegionRequests(applyingRule.Location.ToString(), ref acceptedRequests);
            
            if (bothRules) {
                return IsEligibleCount(applyingRule, ref regionRequests) &&
                       IsEligibleTime(applyingRule, ref regionRequests);
            }

            return applyingRule.Location switch
            {
                LocationEnum.US => IsEligibleCount(applyingRule, ref regionRequests),
                LocationEnum.EU => IsEligibleTime(applyingRule, ref regionRequests),
                _ => false
            };
        }

        #region Private methods

        private void ReadJson()
        {
            using var streamReader = new StreamReader("config.json");
            var json = streamReader.ReadToEnd();
            var data = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, RuleModel>>>(json);
            if (data == null) return;
            
            foreach (var (key, value) in data["Rules"])
            {
                value.Location = (LocationEnum)Enum.Parse(typeof(LocationEnum), key);
                _rules.Add(value);
            }
        }

        private RuleModel? GetLocation(string token) => _rules.FirstOrDefault(rule => token.Contains(rule.Location.ToString()));

        private static List<RequestModel> GetCurrentRegionRequests(string location, ref List<RequestModel> requests)
        {
            return requests
                .Where(request => request.Token.Contains(location))
                .ToList();
        }

        private static bool IsEligibleCount(RuleModel applyingRule, ref List<RequestModel> acceptedRequests)
        {
            return acceptedRequests
                .Count(req => req.Token.Contains(applyingRule.Location.ToString())) < applyingRule.AllowedRequests;
        }

        private static bool IsEligibleTime(RuleModel applyingRule, ref List<RequestModel> acceptedRequests)
        {
            if (acceptedRequests.Count == 0) return true;

            var lastRequest = acceptedRequests
                .OrderBy(request => request.RequestFired)
                .Last();

            var currTime = DateTime.Now;
            var timePassed = (currTime - lastRequest.RequestFired).TotalSeconds;

            return timePassed >= applyingRule.Interval.TotalSeconds && timePassed > 0;
        }

        #endregion
    }
}