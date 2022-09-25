using RateLimiter.Enumerators;
using RateLimiter.Interfaces;
using RateLimiter.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace RateLimiter.Services
{
    public class RateLimiterDefaultEvaluator : IRateLimiterEvaluator
    {
        private IRateLimiterRepository Repository { get; }


        public eRateLimiterResultType Evaluate(RateLimiterHandlerContext context)
        {
            var userToken = context.User.Id.ToString() + context.Policy.Name;

            Repository.SaveUserRequest(userToken, context.RequestDate);

            //Check RateLimits that are defined.
            var rateLimits = context.Policy.RateLimits;

            if (rateLimits.PerSecond > 0)
            {
                var requestCount = Repository.CountRequests(userToken, context.RequestDate, TimeSpan.FromSeconds(1));
                if (requestCount > rateLimits.PerSecond)
                {
                    return eRateLimiterResultType.Deny;
                }
            }

            if (rateLimits.PerMinute > 0)
            {
                var requestCount = Repository.CountRequests(userToken, context.RequestDate, TimeSpan.FromMinutes(1));
                if (requestCount > rateLimits.PerMinute)
                {
                    return eRateLimiterResultType.Deny;
                }
            }

            if (rateLimits.PerHour > 0)
            {
                var requestCount = Repository.CountRequests(userToken, context.RequestDate, TimeSpan.FromHours(1));
                if (requestCount > rateLimits.PerHour)
                {
                    return eRateLimiterResultType.Deny;
                }
            }

            if (rateLimits.MinimumSpan.TotalSeconds > 0)
            {
                var requestCount = Repository.CountRequests(userToken, context.RequestDate, rateLimits.MinimumSpan);
                if (requestCount > 1)
                {
                    return eRateLimiterResultType.Deny;
                }

            }

            return eRateLimiterResultType.Allow;
        }

        public RateLimiterDefaultEvaluator(IRateLimiterRepository repository)
        {
            Repository = repository;
        }
    }
}
