using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using RateLimiter.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RateLimiter.Rules
{
    public class LimitPeriodRule : RequestIntervalRule
    {
        public LimitPeriodRule(IMemoryCache memoryCache)
            : base(memoryCache) { }

        public override string Name { get { return "LimitPeriodRule"; } }
        public override bool IsValid(RateLimitRuleModel rateLimitRuleModel, string token)
        {
            var response = true;

            var key = new RuleDictionaryKey { Rule = Name, Token = token };
            var memoryLimitModel = new TokenInMemoryLimitPeriodModel { };


            _memoryCache.TryGetValue(key, out Dictionary<string, TokenInMemoryLimitPeriodModel>? apiNameDictionary);

            if (apiNameDictionary is null)
            {
                _memoryCache.Set(key, new Dictionary<string, TokenInMemoryLimitPeriodModel>() { { rateLimitRuleModel.Endpoint, memoryLimitModel } });
            }
            else
            {
                var endpoint = rateLimitRuleModel.Endpoint;

                apiNameDictionary.TryGetValue(endpoint, out var limitPeriodModel);

                if (limitPeriodModel is null)
                {
                    apiNameDictionary.Add(endpoint, memoryLimitModel);
                }
                else
                {
                    response = limitPeriodModel.IsAllowed(rateLimitRuleModel.RequestLimit, rateLimitRuleModel.RequestPeriod);
                }

                _memoryCache.Set(key, apiNameDictionary);
            }

            return response;
        }
    }
}
