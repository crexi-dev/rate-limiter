using System;
using System.Collections.Generic;
using System.Linq;
using RateLimiter.Domain.Entities;

namespace RateLimiter.Data.Repository
{
    /// <inheritdoc />
    public class ResourceAccessRepository : IResourceAccessRepository
    {
        /// <inheritdoc />
        public void Add<TResource>(int id, int userId) where TResource : IResource, new()
        {
            string resourceName = new TResource().ResourceName;

            _resourceAccesses.Add(new ResourceAccess() { Accessed = DateTime.Now, ResourceName = resourceName, UserId = userId, Id = id });
        }

        /// <inheritdoc />
        public List<IResourceAccess> Get<TResource>(int id, int userId) where TResource : IResource, new()
        {
            string resourceName = new TResource().ResourceName;

            List<IResourceAccess> result = new();

            result.AddRange(_resourceAccesses.Where(r => r.ResourceName == resourceName && r.Id == id && r.UserId == userId).ToList());

            return result;
        }

        readonly List<IResourceAccess> _resourceAccesses = new();
    }
}