using System;
using System.Linq;
using System.Collections.Generic;
using RateLimiter.Helpers;

namespace RateLimiter
{
    public class RequestValidator
    {
        private static Dictionary<string, List<DateTime>> _requests = new Dictionary<string, List<DateTime>>();
        public bool IsUserAllowed(string accessToken, string apiName)
        {
            if (_requests.ContainsKey(accessToken))
            {
                var userRequests = _requests[accessToken];
                var rules = LimitConfigurator.GetRules(apiName);
                foreach (var rule in rules)
                {
                    //TODO: check if where condition is correct
                    var oldRequestsCount = userRequests.Where(d => d.Add(rule.TimeSpan) >= DateTime.Now).Count();
                    if (oldRequestsCount > rule.RequestsMaxCount)
                        return false;
                }
                userRequests.Add(DateTime.Now);
            }
            else
                _requests.Add(accessToken, new List<DateTime> { DateTime.Now });

            return true;
        }
    }
}
