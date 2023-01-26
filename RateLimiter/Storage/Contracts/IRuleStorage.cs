using System.Collections.Generic;
using RateLimiter.Rule.Contracts;

namespace RateLimiter.Storage.Contracts
{
    public interface IRuleStorage
    {
        List<IRule> GetRule(Models.Enum.ResourceType resourceType);
    }
}