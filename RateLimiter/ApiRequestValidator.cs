using RateLimiter.Rules;
using System.Collections.Generic;

namespace RateLimiter
{
    public class ApiRequestValidator
    {
        private static readonly Dictionary<string, List<ApiRequest>> _requestLog = new Dictionary<string, List<ApiRequest>>();
        private readonly IRateLimitRuleFactory _rateLimitRuleFactory;

        public ApiRequestValidator(IRateLimitRuleFactory rateLimitRuleFactory)
        {
            _rateLimitRuleFactory = rateLimitRuleFactory;
        }

        public bool ValidateRequest(string token, string resourceName)
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

        private List<ApiRequest> GetApiRequestsByToken(string token)
        {
            _requestLog.TryGetValue(token, out List<ApiRequest> existingRequests);

            return existingRequests;
        }
    }
}
