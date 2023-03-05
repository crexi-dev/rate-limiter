using RateLimiter.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RateLimiter.Repositories
{
    public class RuleRepository : IRuleRepository
    {
        public void Add(TimeSpan period, int limit)
        {
            var existing = Store.Store.Rules.FirstOrDefault(x => x.Limit == limit && x.Period == period);
            if (existing != null)
                throw new Exception($"Rule with Period:{period} and Limit:{limit} already exist");

            Store.Store.Rules.Add(RateLimiterRule.Create(period, limit));
        }

        public List<RateLimiterRule> GetAll()
        {
            return Store.Store.Rules.ToList();
        }
    }
}
