using RateLimiter.Models;
using RateLimiter.Rules;

namespace RateLimiter.Repositories
{
    public interface IRequestConverter
    {
        RuleRequestInfo Convert(Request request);
    }
}