using RateLimiter.Application.AccessRestriction.Authorization;
using RateLimiter.Domain.Entities;
using RateLimiter.ViewModels;

namespace RateLimiter.Application.Services
{
    /// <summary>
    /// Service to return resources (in a full app, would support full CRUD)
    /// </summary>
    public interface IResourceService
    {
        /// <summary>
        /// Get a resource by ID, IF the user is allowed to access it right now.
        /// </summary>
        /// <typeparam name="TResource">The domain object to get.</typeparam>
        /// <typeparam name="TViewModel">ViewModel for the domain object.</typeparam>
        /// <param name="id">Domain object id</param>
        /// <param name="userToken">Token identifying user id, region, etc.</param>
        /// <returns></returns>
        TViewModel Get<TResource, TViewModel>(int id, IUserToken userToken) where TResource : IResource, new()
                                                                                                         where TViewModel : IResourceReadViewModelBase;
    }
}