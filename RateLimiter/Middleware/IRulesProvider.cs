using RateLimiter.Models;
using System.Threading.Tasks;

namespace RateLimiter.Middleware
{
    public interface IRulesProvider
    {
        Task<RateLimitRuleOptions> GetConfiguredRulesAsync();
    }
}
