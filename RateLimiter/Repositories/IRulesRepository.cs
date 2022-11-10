using RateLimiter.Enums;
using RateLimiter.Rules;

namespace RateLimiter.Repositories
{
    public interface IRulesRepository
    {
        Rule GetRule(string endpoint, Location location);
    }
}
