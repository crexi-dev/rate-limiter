using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RateLimiter.Contracts.Interfaces;

namespace RateLimiter
{
    public class SimpleApiService : IApiService
    {
        private readonly IRateLimiter _rateLimiter;

        public SimpleApiService(IRateLimiter rateLimiter)
        {
            _rateLimiter = rateLimiter;
        }

        public async Task<string> MakeApiCallAsync(string accessToken, string resource)
        {
            if (!_rateLimiter.IsRequestAllowed(accessToken, resource))
            {
                return "Rate limit exceeded";
            }

            // Simulate an API call
            await Task.Delay(100);
            return "API call successful";
        }
    }
}
