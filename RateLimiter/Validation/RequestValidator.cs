using RateLimiter.ApiRule.Factory;
using RateLimiter.Model.Enum;
using RateLimiter.Model.Request;
using System;
using System.Collections.Generic;
using System.Text;

namespace RateLimiter.Validation
{
    public class RequestValidator
    {
        private static readonly Dictionary<string, List<ApiRequest>> _requestLog = new Dictionary<string, List<ApiRequest>>();
        private readonly IRateLimitedFactory _rateLimitRuleFactory;

        public RequestValidator(IRateLimitedFactory rateLimitRuleFactory)
        {
            _rateLimitRuleFactory = rateLimitRuleFactory;
        }
        private List<ApiRequest> GetApiRequestsByToken(string token)
        {
            _requestLog.TryGetValue(token, out List<ApiRequest> existingRequests);

            return existingRequests;
        }
        public bool ValidateRequest(string token, ResourceEnum resourceName)
        {
            var resourceRules = _rateLimitRuleFactory.GetRateLimitRulesByResource(resourceName);
            var tokenRequestLog = GetApiRequestsByToken(token);


            bool requestGranted = true;

            resourceRules.ForEach(rule =>
            {
                if (requestGranted)
                    requestGranted = rule.Validate(token, resourceName, tokenRequestLog);
            });

            if (!requestGranted)
                return false;

            var apiRequest = new ApiRequest { ResourceName = resourceName };

            if (_requestLog.ContainsKey(token))
            {
                _requestLog[token].Add(apiRequest);
            }
            else
            {
                _requestLog.Add(token, new List<ApiRequest> { apiRequest });
            }

            return true;
        }

        public int GetApiRequestCountByToken(string token)
        {
            var requests = GetApiRequestsByToken(token);

            return requests?.Count ?? 0;
        }

    }
}
