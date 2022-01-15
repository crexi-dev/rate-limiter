using RateLimiter.Domain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RateLimiter.RateLimiter.Rules
{
    public sealed class RequestTimeAfterLastPolicy : IRateLimitPolicy
    {
        private readonly TimeSpan timeAfterLastCall;

        public RequestTimeAfterLastPolicy(TimeSpan timeAfterLastCall)
        {
            this.timeAfterLastCall = timeAfterLastCall;
        }

        public bool Check(string accessToken, IEnumerable<UserRequest> userRequests, DateTime currentDate)
        {
            if (userRequests is null || string.IsNullOrEmpty(accessToken))
                return true;

            var lastCall = userRequests
                .Where(r => r.AccessToken == accessToken)
                .OrderByDescending(r => r.Date)
                .FirstOrDefault();

            if (lastCall is null)
                return true;

            var dateLimit = currentDate.Subtract(timeAfterLastCall);

            return dateLimit < lastCall.Date;
        }
    }
}
