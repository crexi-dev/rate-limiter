using RateLimiter.RateLimiter.Services;
using System;
using System.Threading.Tasks;

namespace RateLimiter.Api
{
    public sealed class SecureApiEndpoint<T> : IApiEndpoint<T>
    {
        private readonly IApiEndpoint<T> apiEndpoint;
        private readonly IRateLimitService rateLimitService;

        public SecureApiEndpoint(
            IApiEndpoint<T> apiEndpoint,
            IRateLimitService rateLimitService)
        {
            this.apiEndpoint = apiEndpoint;
            this.rateLimitService = rateLimitService;
        }

        public async Task<Response<T>> ActionAsync(string accessToken)
        {
            if(await rateLimitService.ValidateAsync(accessToken, apiEndpoint.GetType().Name))
                return await apiEndpoint.ActionAsync(accessToken);

            throw new InvalidOperationException("Rate limit policy failure.");
        }
    }
}
