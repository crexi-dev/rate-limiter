using RateLimiter.Application.AccessRestriction.Authorization;
using RateLimiter.Domain.Entities;

namespace RateLimiter.Application.AccessRestriction.Rule
{
    /// <summary>
    /// Base class for specific rule implementations.
    /// Implements the <see cref="IRule" />
    /// </summary>
    /// <seealso cref="IRule" />
    public abstract class Rule : IRule
    {
        ///<inheritdoc/>
        public string FailMessage { get; set; }

        ///<inheritdoc/>
        public int Id { get; set; }

        ///<inheritdoc/>
        public string Name { get; set; }

        ///<inheritdoc/>
        public IRuleResult Execute<TResource>(int id, IUserToken userToken) where TResource : IResource, new()
        {
            IRuleResult ruleResult = new RuleResult(this);

            try
            {
                ruleResult = ExecuteRule<TResource>(id, userToken);
            }
            catch (System.Exception ex)
            {
                ruleResult.IsSuccess = false;
                ruleResult.FailMessage = ex.Message;
            }

            return ruleResult;
        }

        /// <summary>
        /// Executes the rule. This is where implementing class would do its logic, for a specific rule type.
        /// </summary>
        /// <param name="id">Id of the resource to access.</param>
        /// <param name="userToken">The access identity, a token containing info about the calling consumer.</param>
        /// <returns>IRuleResult.</returns>
        protected abstract IRuleResult ExecuteRule<TResource>(int id, IUserToken userToken) where TResource : IResource, new();
    }
}
