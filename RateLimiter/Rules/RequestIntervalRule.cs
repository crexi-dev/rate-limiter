using Microsoft.Extensions.Caching.Memory;
using RateLimiter.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RateLimiter.Rules
{
    public class RequestIntervalRule : RateLimitRule
    {
        public RequestIntervalRule(IMemoryCache memoryCache)
            : base(memoryCache)
        { }

        public override string Name { get { return "RequestIntervalRule"; } }

        public override bool IsValid(RateLimitRuleModel rateLimitRuleModel, string token)
        {
            var respose = true;

            var key = new RuleDictionaryKey { Rule = Name, Token = token };
            var tokenIntervalModel = new TokenInMemoryIntervalModel { };

            _memoryCache.TryGetValue(key, out Dictionary<string, TokenInMemoryIntervalModel>? apiNameDictionary);

            if (apiNameDictionary is null)
            {
                _memoryCache.Set(key, new Dictionary<string, TokenInMemoryIntervalModel>() { { rateLimitRuleModel.Endpoint, tokenIntervalModel } });
            }
            else
            {
                var endpoint = rateLimitRuleModel.Endpoint;

                apiNameDictionary.TryGetValue(endpoint, out var intervalModel);

                if (intervalModel is null)
                {
                    apiNameDictionary.Add(endpoint, tokenIntervalModel);
                }
                else
                {
                    respose = intervalModel.IsAllowed(rateLimitRuleModel.RequestPeriod);
                }

                _memoryCache.Set(key, apiNameDictionary);
            }

            return respose;
        }
    }
}
