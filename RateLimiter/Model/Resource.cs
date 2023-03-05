using System.Collections.Generic;
using System.Linq;

namespace RateLimiter.Model
{
    public class Resource
    {
        public Resource(string path)
        {
            Path = path;
            Rules = new List<RateLimiterRule>();
        }

        public string Path { get; private set; }

        public List<RateLimiterRule> Rules { get; private set; }

        public void AddRule(RateLimiterRule rule)
        {
            var existingRule = Rules.FirstOrDefault(x => x.Period == rule.Period && x.Limit == rule.Limit);

            if (existingRule != null)
                throw new System.Exception($"Resource with Path :{Path} already contain rule with Limit:{rule.Limit} and Period:{rule.Period}");

            Rules.Add(rule);
        }

        public static Resource Create(string path) => new Resource(path);
    }
}
