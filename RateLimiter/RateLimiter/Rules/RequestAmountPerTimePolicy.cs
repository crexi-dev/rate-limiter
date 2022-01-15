using RateLimiter.Domain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RateLimiter.RateLimiter.Rules
{
    public sealed class RequestAmountPerTimePolicy : IRateLimitPolicy
    {
        private readonly int amount;
        private readonly TimeSpan time;

        public RequestAmountPerTimePolicy(int amount, TimeSpan time)
        {
            this.amount = amount;
            this.time = time;
        }

        public bool Check(string accessToken, IEnumerable<UserRequest> userRequests, DateTime currentDate)
        {
            if (userRequests is null || string.IsNullOrEmpty(accessToken))
                return true;

            var dateFrom = currentDate.Subtract(time);
            var requestsAmount = userRequests
                .Where(r => r.AccessToken == accessToken && r.Date >= dateFrom)
                .Count();

            return requestsAmount < amount;
        }
    }
}
