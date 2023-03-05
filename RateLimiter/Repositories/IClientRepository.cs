using RateLimiter.Model;
using System.Collections.Generic;

namespace RateLimiter.Repositories
{
    public interface IClientRepository
    {
        void Add(string clientType);
        void AddResources(string clientType, List<Resource> resources);
        void AddResource(string clientType, Resource resource);

        List<RateLimiterRule> GetRulesOfResource(string clientType, string path);
    }
}
