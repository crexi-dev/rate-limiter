using RateLimiter.Core;
using System.Threading.Tasks;

namespace RateLimiter.Rules.Interfaces
{
    public interface IRateLimiterRule
    {
        public Task<bool> IsRequestAllowedAsync(ClientRequestContext context);
    }

}
