using Microsoft.AspNetCore.Http;
using RateLimiter.Interfaces.Factories;
using RateLimiter.Interfaces.Models;
using System.Threading.Tasks;

namespace RateLimiter.Interfaces.Business
{
    /// <summary>
    /// Describes a collaborator responsible for verifying that an incoming request complies with
    /// a set of configured rate-limiting rules
    /// </summary>
    public interface IRateLimitRulesEngine
    {
        /// <summary>
        /// Called from a consumer to validate an HTTP request against configured
        /// rate-limiting rules
        /// </summary>
        /// <param name="context">The HTTP context which contains the HTTP request</param>
        /// <param name="user">The user associated with the request</param>
        /// <returns>An awaitable result with a boolean indicator an optional error message</returns>
        Task<IRateLimitRuleResult> Run(HttpContext context, IUser user);

        /// <summary>
        /// Allows consumers to supply a factory responsible for creating their own custom rate-limiting rules
        /// </summary>
        /// <param name="rulesFactory">A custom IRateLimitRuleFactory</param>
        void AddRulesFactory(IRateLimitRuleFactory rulesFactory);
    }
}
