using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;
using RateLimiter.Models;
using RateLimiter.Models.Enums;

namespace RateLimiter.Services
{
    public static class RulesService
    {
        public static List<RuleModel> GetRules()
        {
            List<RuleModel> rules = new List<RuleModel>();

            RuleModel usRule = new RuleModel();
            usRule.RequestFrequency = 50;
            usRule.FromLastCallTimePassed = new TimeSpan(1000);

            RuleModel euRule = new RuleModel();
            usRule.RequestFrequency = 30;
            usRule.FromLastCallTimePassed = new TimeSpan(500);

            rules.Add(usRule);
            rules.Add(euRule);

            return rules;
        }

    }
}
