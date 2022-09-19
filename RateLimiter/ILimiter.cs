using RateLimiter.Rules;

namespace RateLimiter
{
    /// <summary>
    /// The interface for limiter. Can be used instead of processor, if return type support Error description.
    /// </summary>
    public interface ILimiter : IRequestProcessor
    {
        void AddRule(ILimiteRule rule);
    }
}
