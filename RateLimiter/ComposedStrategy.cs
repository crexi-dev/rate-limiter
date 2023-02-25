using System.Collections.Generic;

namespace RateLimiter
{
    public class ComposedStrategy : IRateLimitStrategy
    {
        private readonly TimespanRateLimitStrategy strategy1;
        private readonly TimespanPassedSinceTheLastStrategy strategy2;
        Dictionary<string, IRateLimitStrategy> strategyMap; 
        public ComposedStrategy(TimespanRateLimitStrategy strategy1, TimespanPassedSinceTheLastStrategy strategy2)
        {
            this.strategy1 = strategy1;
            this.strategy2 = strategy2;
            strategyMap =new Dictionary<string, IRateLimitStrategy>() {
                { "US",this.strategy1},
                { "EU",this.strategy2}
            };
        }

        public bool IsRequestAllowed(string username, string resource, IRequestRepository requestRepository, string region)
        {
            return strategyMap[region].IsRequestAllowed(username,resource,requestRepository,region);
        }
    }
}
