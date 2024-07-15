 
namespace RateLimiter
{
    /// <summary>
    /// Base class for common rate limiter functionalities
    /// </summary>
    public abstract class RateLimitRuleBase : IRateLimitRule
    {
        /// <summary>
        /// Checks if number of requests exeeded limit
        /// </summary>
        /// <param name="endpoint">API endpoint path to enforce rate limit on. i.e. /api/health</param>
        /// <param name="clientIdentifier">ID of the client that is calling endpoint</param>
        /// <returns></returns>
        public abstract bool IsLimitExceeded(string endpoint, string clientIdentifier);
        /// <summary>
        /// Records requests to be used in rate limiting decision
        /// </summary>
        /// <param name="endpoint">API endpoint path to enforce rate limit on. i.e. /api/health</param>
        /// <param name="clientIdentifier">ID of the client that is calling endpoint</param>
        public abstract void RecordRequest(string endpoint, string clientIdentifier);
    }
}
