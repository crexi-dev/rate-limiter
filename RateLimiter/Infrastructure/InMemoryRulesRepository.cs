using RateLimiter.Domain.Resource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
