using RateLimiter.Domain.Entities;

namespace RateLimiter.Data.Repository
{
    /// <summary>
    /// In memory storage of resources
    /// </summary>
    public interface IResourceRepository
    {
        /// <summary>
        /// Add a resource of a specific type.
        /// </summary>
        /// <typeparam name="TResource"></typeparam>
        /// <param name="resource"></param>
        void Add<TResource>(IResource resource) where TResource : IResource, new();

        /// <summary>
        /// get a resource of a specific type and id.
        /// </summary>
        /// <typeparam name="TResource"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        IResource Get<TResource>(int id) where TResource : IResource, new();
    }
}