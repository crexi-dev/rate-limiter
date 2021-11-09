namespace RateLimiter.Application.AccessRestriction.Rule.RateLimit
{
    /// <summary>
    /// Rule where access to a resource is defined in terms of how long must elapse between accesses
    /// Implements the <see cref="RateLimiter.Application.AccessRestriction.Rule.IRule" />
    /// </summary>
    /// <seealso cref="RateLimiter.Application.AccessRestriction.Rule.IRule" />
    public interface ITimeElapsedRule : IRule
    {
        /// <summary>
        /// Gets or sets the minimum seconds elapsed between views.
        /// </summary>
        int MinimumSecondsElapsed { get; set; }
    }
}