using Microsoft.Extensions.Caching.Memory;
using RateLimiter.RateLimitRules;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using RateLimiter.Repository;
using RateLimiter.Models;

namespace RateLimiter
{
    public class RulesConfigService : IRulesConfigService
    {
        private readonly IRulesRepository _rulesRepository;
        public RulesConfigService(IRulesRepository rulesRepository)
        {
            _rulesRepository = rulesRepository;
        }

        public void SetRules(string key, List<IRateLimitRule> rules)
        {
            _rulesRepository.AddOrReplace(new RuleCollection(key)
            { 
                RateLimitRules = rules 
            });

        }

    }
}
