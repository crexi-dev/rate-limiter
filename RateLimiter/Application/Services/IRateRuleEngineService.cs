using RateLimiter.Application.AccessRestriction.Authorization;
using RateLimiter.Application.AccessRestriction.Rule;
using RateLimiter.Domain.Entities;

namespace RateLimiter.Application.Services
{
    /// <summary>
    /// Engine for executing rate limiting rules.
    /// </summary>
    public interface IRateRuleEngineService
    {
        /// <summary>
        /// Execute all rules for a given resource type, id and user
        /// </summary>
        /// <typeparam name="TResource"></typeparam>
        /// <param name="id"></param>
        /// <param name="userToken"></param>
        /// <returns></returns>
        IRuleEngineExecutionResult Execute<TResource>(int id, IUserToken userToken)
            where TResource : IResource, new();
    }
}