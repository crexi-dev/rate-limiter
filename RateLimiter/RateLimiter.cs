using System;
using System.Threading.Tasks;

namespace RateLimiter
{
    public class RateLimiter : IRateLimiter
    {
        public Task<bool> Check(string resource, string token)
        {
            return Task.FromResult(true);
        }
    }
}