using RateLimiter.Application.AccessRestriction.Rule;
using RateLimiter.Domain.Entities;

namespace RateLimiter.Data.Repository
{
    /// <summary>
    /// In memory collection of rules
    /// </summary>
    public interface IRuleRepository
    {
        /// <summary>
        /// Get all rules for a resource type
        /// </summary>
        /// <typeparam name="TResource"></typeparam>
        /// <returns></returns>
        IRuleSet GetAll<TResource>() where TResource : IResource, new();

        /// <summary>
        /// Add a rule for a resource type
        /// </summary>
        /// <typeparam name="TResource"></typeparam>
        /// <param name="rule"></param>
        void Add<TResource>(IRule rule) where TResource : IResource, new();
    }
}