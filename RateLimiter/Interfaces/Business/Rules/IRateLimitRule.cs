using Microsoft.AspNetCore.Http;
using RateLimiter.Interfaces.Models;
using System;
using System.Threading.Tasks;

namespace RateLimiter.Interfaces.Business.Rules
{
    /// <summary>
    /// Defines a rate limit rule
    /// </summary>
    public interface IRateLimitRule
    {
        /// <summary>
        /// Verify that the rule is not violated
        /// </summary>
        /// <param name="context">The HttpContext of the HTTP request</param>
        /// <param name="user">The user associated with the request</param>
        /// <param name="endpoint">The endpoint the rule is applied against</param>
        /// <param name="requestTime">The time of the request shared across all rules</param>
        /// <returns>An awaitable with result type IRateLimitRuleResult</returns>
        Task<IRateLimitRuleResult> Verify(HttpContext context, IUser user,
            IEndpoint endpoint, DateTime requestTime);
    }
}
