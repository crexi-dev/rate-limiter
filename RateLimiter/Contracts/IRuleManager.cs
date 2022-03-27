using RateLimiterMy.Contracts;
using RateLimiterMy.Models;
using System.Collections.Generic;

namespace RateLimiterMy.Contracts
{
    public interface IRuleManager
    {
        void AddRegionRule(Location local, IRule rule);
        void AddResourcesRule(string resourcesName, IRule rule);
        ICollection<IRule> GetCurrentRules(IRequest request);
        bool Validate(IRequest request);
    }
}