namespace RateLimiter.Application.AccessRestriction.Rule.RateLimit
{
    /// <summary>
    /// Rule where access to a resource is defined in terms of how many times per minute a resource can be accessed
    /// Implements the <see cref="RateLimiter.Application.AccessRestriction.Rule.IRule" />
    /// </summary>
    /// <seealso cref="RateLimiter.Application.AccessRestriction.Rule.IRule" />
    public interface IPerMinuteRule : IRule
    {
        /// <summary>
        /// Gets or sets the allowed accesses per minute.
        /// </summary>
        int AccessPerMinute { get; set; }
    }
}