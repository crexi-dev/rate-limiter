using Microsoft.Extensions.Caching.Memory;
using RateLimiter.Factories;
using RateLimiter.Models;
using RateLimiter.Models.Configuration;
using RateLimiter.Stores;

namespace RateLimiter
{
    public sealed class RateLimiter : IRateLimiter
    {
        private readonly PoliciesStore policiesStore;
        private readonly СlientStatisticsStore clientStatisticsStore;

        public RateLimiter(IRateLimiterConfiguration configuration, IMemoryCache cache)
        {
            var resourcePolicies = new ResourcePoliciesFactory().Create(configuration);

            policiesStore = new PoliciesStore(resourcePolicies);
            clientStatisticsStore = new СlientStatisticsStore(cache);
        }

        public RateLimiterResult Execute(RateLimiterContext context)
        {
            var clientStatistics = clientStatisticsStore.Get(context.ClientId, context.ResourceName);
            var policies = policiesStore.GetPolicies(context.ResourceName);

            var rateLimiterResult = new RateLimiterResult();

            foreach (var policy in policies)
            {
                var policyResult = policy.Execute(clientStatistics, context);

                rateLimiterResult.Record(policyResult);

                if (rateLimiterResult.IsRateLimited == true)
                    break;
            }

            clientStatistics.Update(rateLimiterResult);
            clientStatisticsStore.Set(clientStatistics);

            return rateLimiterResult;
        }
    }
}
