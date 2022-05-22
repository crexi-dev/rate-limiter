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
    public class RequestIntervalRule : IRule
    {
        private readonly ITokenIntervalRepository _tokenIntervalRepository;

        public RequestIntervalRule()
        {
            _tokenIntervalRepository = new TokenIntervalRepository();
        }

        public bool Evaluate(RuleSettingModel ruleData, HttpContext httpContext)
        {
            //find if there are rules for this token
            string token = httpContext.Request.Headers["Authorization"];
            if (string.IsNullOrEmpty(token))
                throw new Exception($"Endpoint: {ruleData.Endpoint} does not have valid token authorization");

            if (string.IsNullOrEmpty(ruleData.Endpoint))
                throw new Exception($"No Valid Endpoint is Specified for Rule: {ruleData.Name}");

            var tokenRules = _tokenIntervalRepository.GetRulesForEndpoint(token, ruleData.Endpoint);
            if (tokenRules != null)
            {
                List<bool> ruleResults = new List<bool>();
                foreach (var rule in tokenRules)
                {
                    ruleResults.Add(rule.allowRequest());
                }

                return !ruleResults.Contains(false);

            }
            else //create new interval rule for this token
            {
                TokenIntervalModel newTokenInterval = new TokenIntervalModel(ruleData.Period);
                _tokenIntervalRepository.AddNewTokenInterval(token, ruleData.Endpoint, newTokenInterval);
                return newTokenInterval.allowRequest();
            }

        }
    }
}
