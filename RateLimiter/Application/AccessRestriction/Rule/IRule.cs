using RateLimiter.Application.AccessRestriction.Authorization;
using RateLimiter.Domain.Entities;

namespace RateLimiter.Application.AccessRestriction.Rule
{
    /// <summary>
    /// Basic framework of a rate limiting rule.
    /// </summary>
    public interface IRule
    {
        /// <summary>
        /// Client friendly message indicating why the rule failed.
        /// </summary>
        public string FailMessage { get; set; }

        /// <summary>
        /// Identifier for this rule.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Execute this rule
        /// </summary>
        /// <typeparam name="TResource">The resource type that this rule is for.</typeparam>
        /// <param name="id"></param>
        /// <param name="userToken"></param>
        /// <returns></returns>
        public IRuleResult Execute<TResource>(int id, IUserToken userToken) where TResource : IResource, new();
    }
}
