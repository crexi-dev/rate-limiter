namespace RateLimiter.Interfaces.Models
{
    /// <summary>
    /// Defines result information for a rule run through the rate-limiting rules engine
    /// </summary>
    public interface IRateLimitRuleResult
    {
        /// <summary>
        /// Indicates whether or not the pipeline should proceed
        /// </summary>
        bool Proceed { get; }

        /// <summary>
        /// An optional error
        /// </summary>
        string? Error { get; }
    }
}
