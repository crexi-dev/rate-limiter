using System;
using System.Collections.Generic;
using System.Linq;
using RateLimiter.Application.AccessRestriction.Authorization;
using RateLimiter.Data.Repository;

namespace RateLimiter.Application.AccessRestriction.Rule.RateLimit
{
    /// <inheritdoc cref="IPerMinuteRule" />
    public class PerMinuteRule : TimeRuleBase, IPerMinuteRule
    {
        private readonly IResourceAccessRepository _resourceAccessRepository;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="resourceAccessRepository"></param>
        public PerMinuteRule(IResourceAccessRepository resourceAccessRepository)
        {
            _resourceAccessRepository = resourceAccessRepository;
        }

        ///<inheritdoc/>
        public int AccessPerMinute { get; set; }

        ///<inheritdoc/>
        protected override IRuleResult ExecuteRule<TResource>(int id, IUserToken userToken)
        {
            RuleResult ruleResult = new(this);

            DateTime lowerBound = DateTime.Now.AddMinutes(-1);

            List<IResourceAccess> accesses = _resourceAccessRepository.Get<TResource>(id, userToken.UserId).Where(a => a.Accessed >= lowerBound).ToList();

            if (accesses.Count + 1 > AccessPerMinute)
            {
                ruleResult.IsSuccess = false;
                ruleResult.FailMessage = $"Accessed more than {AccessPerMinute} in the last minute.";
            }
            else
            {
                ruleResult.IsSuccess = true;
            }

            return ruleResult;
        }
    }
}