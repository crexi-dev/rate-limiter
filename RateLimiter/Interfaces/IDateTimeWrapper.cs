using System;

namespace RateLimiter.Interfaces
{
    /// <summary>
    /// Represents a wrapper for date and time operations, allowing for testability.
    /// </summary>
    public interface IDateTimeWrapper
    {
        /// <summary>
        /// Gets the current UTC date and time.
        /// </summary>
        DateTime UtcNow { get; }
    }

}
