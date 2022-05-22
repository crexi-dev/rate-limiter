using Microsoft.AspNetCore.Http;
using RateLimiter.Interfaces.Repositories;
using RateLimiter.Interfaces.Rules;
using RateLimiter.Models;
using RateLimiter.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter.Rules
{
    public class LimitPerTimespanRule : IRule
    {
        private readonly ITokenBucketRepository _tokenBucketRepository;

        public LimitPerTimespanRule()
        {
            _tokenBucketRepository = new TokenBucketRepository();
        }
        public bool Evaluate(RuleSettingModel ruleData, HttpContext httpContext)
        {
            //find if there are rules for this token
            string token = httpContext.Request.Headers["Authorization"];

            if (string.IsNullOrEmpty(token))
                throw new Exception($"Endpoint: {ruleData.Endpoint} does not have valid token authorization");

            if (string.IsNullOrEmpty(ruleData.Endpoint))
                throw new Exception($"No Valid Endpoint is Specified for Rule: {ruleData.Name}");

            var tokenRules = _tokenBucketRepository.GetRulesForEndpoint(token, ruleData.Endpoint);
            if (tokenRules!=null) 
            {
                List<bool> ruleResults = new List<bool>();
                foreach(var rule in tokenRules)
                {
                    ruleResults.Add(rule.allowRequest());
                }

                return !ruleResults.Contains(false);

            }
            else //create new bucket for this token
            {
                TokenBucketModel newTokenBucket = new TokenBucketModel(ruleData.Limit, ruleData.Period);
                _tokenBucketRepository.AddNewTokenBucket(token, ruleData.Endpoint, newTokenBucket);
                return newTokenBucket.allowRequest();
            }
        }
    }
}
