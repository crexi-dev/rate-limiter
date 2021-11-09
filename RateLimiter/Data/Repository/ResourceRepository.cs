using System.Collections.Generic;
using System.Linq;
using RateLimiter.Domain.Entities;

namespace RateLimiter.Data.Repository
{
    /// <inheritdoc />
    public class ResourceRepository : IResourceRepository
    {
        /// <inheritdoc />
        public void Add<TResource>(IResource resource) where TResource : IResource, new()
        {
            Resources.Add(resource);
        }

        /// <inheritdoc />
        public IResource Get<TResource>(int id) where TResource : IResource, new()
        {
            string resourceName = new TResource().ResourceName;

            return Resources.FirstOrDefault(r => r.ResourceName == resourceName && r.Id == id);
        }

        private List<IResource> Resources { get; } = new();
    }
}