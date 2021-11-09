using RateLimiter.Application.AccessRestriction.Authorization;
using RateLimiter.Application.Services;
using RateLimiter.Domain.Entities;
using RateLimiter.ViewModels;

namespace RateLimiter.Presentation
{
    /// <summary>
    /// Simulated WebAPI controller.
    /// </summary>
    public class Api
    {
        private readonly IResourceService _resourceService;

        /*
        Note: Keeping this simple per instructions. In a full app, the layers for an API call would be like:

        API - Accepts request view model (or in this case, just an id), calls ResourceService
            - Resource service determines if resource can be served via rule engine
                - if no, resource throws a RateLimitException to API, which returns appropriate response to client
                - if yes, call data repository, map response to view model, and pass back up chain to API

        "Controller" methods intentionally don't handle exceptions thrown, these would normally bubble up to an ExceptionFilter.
        */

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="resourceService"></param>
        public Api(IResourceService resourceService)
        {
            _resourceService = resourceService;
        }

        /// <summary>
        /// Get a viewModel representing an instance of ResourceA
        /// </summary>
        /// <param name="id">Unique ID for the instance of the resource.</param>
        /// <param name="userToken">Token identifying the user.</param>
        /// <returns></returns>
        public ResourceAReadViewModel GetResourceA(int id, UserToken userToken)
        {
            return GetResource<ResourceA, ResourceAReadViewModel>(id, userToken);
        }

        /// <summary>
        /// Get a viewModel representing an instance of ResourceB
        /// </summary>
        /// <param name="id">Unique ID for the instance of the resource.</param>
        /// <param name="userToken">Token identifying the user.</param>
        /// <returns></returns>
        public ResourceBReadViewModel GetResourceB(int id, UserToken userToken)
        {
            return GetResource<ResourceB, ResourceBReadViewModel>(id, userToken);
        }

        private TViewModel GetResource<TResource, TViewModel>(int id, UserToken userToken) where TResource : IResource, new() where TViewModel : IResourceReadViewModelBase
        {

            return _resourceService.Get<TResource, TViewModel>(id, userToken);
            // In a real WebAPI, return with a 200


            /*
              Note: No try/catch, if this were a real web API, we could let the exceptions bubble
              to an ExceptionFilter handler that would return the correct HTTP response (which is
              usually going to be a 429 for rate limiting, 404 or 500 for most other conditions)
            */
        }
    }
}
