using System.Collections.Generic;
using System.Linq;
using RateLimiter.Application.AccessRestriction.Rule;
using RateLimiter.Domain.Entities;

namespace RateLimiter.Data.Repository
{
    /// <inheritdoc />
    public class RuleRepository : IRuleRepository
    {
        /// <inheritdoc />
        public IRuleSet GetAll<TResource>() where TResource : IResource, new()
        {
            string resourceName = new TResource().ResourceName;

            return new RuleSet()
            {
                Name = $"Active rules for resource {resourceName}",
                Description = $"Active rules for resource {resourceName}",
                Rules = RuleResources.FindAll(r => r.ResourceName == resourceName).Select(r => r.Rule).ToList()
            };
        }

        /// <inheritdoc />
        public void Add<TResource>(IRule rule) where TResource : IResource, new()
        {
            RuleResources.Add(new RuleResource() { ResourceName = new TResource().ResourceName, Rule = rule });
        }

        private List<IRuleResource> RuleResources { get; } = new List<IRuleResource>();
    }
}