using RateLimiter.Model;
using System.Collections.Generic;

namespace RateLimiter.Repositories
{
    public interface IResourceRepository
    {
        void Add(string resource);
        List<Resource> GetAll();
        void AddRulesToResource(string path, List<RateLimiterRule> rules);
        void AddRuleToResource(string path, RateLimiterRule rule);

        List<RateLimiterRule> GetRulesOfResource(string path);
    }
}
