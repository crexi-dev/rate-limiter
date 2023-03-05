using RateLimiter.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RateLimiter.Repositories
{
    public class ResourceRepository : IResourceRepository
    {
        public void Add(string resource)
        {
            Store.Store.Resources.Add(Resource.Create(resource));
        }

        public List<Resource> GetAll()
        {
            return Store.Store.Resources.ToList();
        }

        public List<RateLimiterRule> GetRulesOfResource(string path)
        {
            var resource = Store.Store.Resources.FirstOrDefault(x => x.Path == path);

            if (resource == null)
                throw new Exception($"Resource with Path:{path} does not exist");

            return resource.Rules;
        }

        public void AddRulesToResource(string path, List<RateLimiterRule> rules)
        {
            foreach (var rule in rules)
            {
                this.AddRuleToResource(path, rule);
            }
        }

        public void AddRuleToResource(string path, RateLimiterRule rule)
        {
            var resource = Store.Store.Resources.FirstOrDefault(x => x.Path == path);

            if (resource == null)
                throw new Exception($"Resource with Path:{path} does not exist");

            resource.AddRule(rule);
        }
    }
}
