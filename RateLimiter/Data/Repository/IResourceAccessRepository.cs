using System.Collections.Generic;
using RateLimiter.Domain.Entities;

namespace RateLimiter.Data.Repository
{
    /// <summary>
    /// Interface IResourceAccessRepository, read/write collection of views of resources
    /// </summary>
    public interface IResourceAccessRepository
    {

        /// <summary>
        /// Add a view of a resource by a user.
        /// </summary>
        /// <typeparam name="TResource">The type of the t resource.</typeparam>
        /// <param name="id">The identifier.</param>
        /// <param name="userId">The user identifier.</param>
        void Add<TResource>(int id, int userId) where TResource : IResource, new();

        /// <summary>
        /// Gets all views of a resource by a user id.
        /// </summary>
        /// <typeparam name="TResource">The type of the t resource.</typeparam>
        /// <param name="id">The identifier.</param>
        /// <param name="userId">The user identifier.</param>
        /// <returns>List&lt;IResourceAccess&gt;.</returns>
        List<IResourceAccess> Get<TResource>(int id, int userId) where TResource : IResource, new();
    }
}