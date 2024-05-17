using RateLimiter.Rules;

namespace RateLimiter.Repositories
{
    public record ValidateReadyRule (IRule Rule, IRequestConverter RequestConverter);
    
}