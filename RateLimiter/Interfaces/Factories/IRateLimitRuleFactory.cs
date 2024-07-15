using RateLimiter.Interfaces.Business.Rules;
using RateLimiter.Interfaces.Configuration;

namespace RateLimiter.Interfaces.Factories
{
    /// <summary>
    /// Responsbile for creating IRateLimitRules
    /// </summary>
    public interface IRateLimitRuleFactory
    {
        /// <summary>
        /// Checks if the string name of a type is support by the factory
        /// </summary>
        /// <param name="type">Class name of the type to check</param>
        /// <returns>True if the type is supported, false otherwise</returns>
        bool SupportsType(string type);

        /// <summary>
        /// Creates an IRateLimitRule 
        /// </summary>
        /// <param name="configuration">An IRateLimitRuleConfiguration object supplid via appsettings configuration</param>
        /// <returns>An IRateLimitRule instance</returns>
        IRateLimitRule Create(IRateLimitRuleConfiguration configuration);
    }
}
