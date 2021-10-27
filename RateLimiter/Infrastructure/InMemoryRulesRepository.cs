using RateLimiter.Domain.Resource;

namespace RateLimiter.Infrastructure
{
    public class InMemoryRulesRepository : IInMemoryRulesRepository
    {
        private ResourceRules _resourceRules;
        public ResourceRules ResourceRules => _resourceRules;

        public InMemoryRulesRepository(ResourceRules resourceRules)
        {
            _resourceRules = resourceRules;
        }
    }
}
