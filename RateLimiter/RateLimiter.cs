using System;
using System.Linq;
using System.Collections.Generic;

using RateLimiter.Rules;

namespace RateLimiter
{
    internal class RateLimiter
    {
        private readonly RequestsHistory _requestsHistory;
        private readonly IEnumerable<IRateLimiterRule> _rules;

        public RateLimiter(RequestsHistory requestsHistory, IRateLimiterRule rule) : this(requestsHistory, new[] { rule })
        { }

        public RateLimiter(RequestsHistory requestsHistory, IEnumerable<IRateLimiterRule> rules)
        {
            _requestsHistory = requestsHistory;
            _rules = rules;
        }

        public bool IsAllowed(string userToken, DateTimeOffset requestDateTime)
        {
            var userRequests = _requestsHistory.GetRequests(userToken);

            var isAllowed = _rules.All(r => r.IsAllowed(userRequests, requestDateTime));

            if (isAllowed) _requestsHistory.AcceptRequest(userToken, requestDateTime);

            return isAllowed;
        }
    }
}