using RateLimiter.Data;
using RateLimiter.Model.Resource;
using RateLimiter.Model.Resource.Config;
using System;

namespace RateLimiter.Resource
{
    public class ServiceResourceConfigLoader : IServiceResourceConfigLoader
    {
        private readonly RepositoryRateLimiter Repo;

        public ServiceResourceConfigLoader(RepositoryRateLimiter repo)
        {
            Repo = repo;
        }        

        public ResourceConfig GetConfig(ResourceRequest request)
        {
            ValidateRequest(request);

            var config = Repo.GetResourceConfig(request.Resource);

            if (config == null)
                throw new Exception("Unable to load resource configuration");

            return config;
        }

        private void ValidateRequest(ResourceRequest request)
        {
            if (request == null)
                throw new ArgumentException("A resource request is required");

            if (request.Resource == EnumResource.NotSet)
                throw new ArgumentException("Please provide the resource you are trying to validate");

            if (string.IsNullOrWhiteSpace(request.Token))
                throw new ArgumentException("Please provide a valid client token");
        }
    }
}
